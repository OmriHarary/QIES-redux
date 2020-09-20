using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QIES.Api.Models;
using QIES.Api.Models.Validation;
using QIES.Common;
using QIES.Common.Record;
using QIES.Core;
using QIES.Core.Commands;
using QIES.Core.Services;
using QIES.Core.Users;

using static System.Net.Mime.MediaTypeNames;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace QIES.Web.Controllers
{
    [ApiController]
    [Consumes(Application.Json)]
    [Produces(Application.Json)]
    [Route("api/v1/[controller]")]
    public class ServicesController : ControllerBase
    {
        private readonly ILogger<ServicesController> logger;
        private readonly IServicesList servicesList;
        private readonly IUserManager userManager;

        public ServicesController(
            ILogger<ServicesController> logger,
            IServicesList servicesList,
            IUserManager userManager)
        {
            this.logger = logger;
            this.servicesList = servicesList;
            this.userManager = userManager;
        }

        // [HttpGet]
        // public async Task<ActionResult<IEnumerable<Service>>> GetServices()
        // {
        //     return new List<Service>();
        // }

        [HttpGet("{id}")]
        public async Task<ActionResult<Service>> GetService([ServiceNumber] string id)
        {
            var serviceNumber = new ServiceNumber(id);

            if (servicesList.IsInList(serviceNumber))
            {
                return Ok();
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<TransactionRecord>> CreateService(
            CreateServiceRequest request,
            [FromServices] ITransaction<CreateServiceRequest> transaction)
        {
            logger.LogInformation("CreateService requested for {serviceNumber}", request.ServiceNumber);

            if (request.UserId is Guid userId && userManager.IsLoggedIn(userId))
            {
                if (userManager.UserType(userId) != LoginType.Planner)
                {
                    logger.LogWarning("CreateService requested by non-planner user {userId}", userId);
                    return Forbid();
                }

                var serviceNumber = new ServiceNumber(request.ServiceNumber);

                if (servicesList.IsInList(serviceNumber))
                {
                    logger.LogWarning("Could not create service. Requested service number, {serviceNumber}, already allocated", serviceNumber);
                    return Conflict();
                }

                var record = await transaction.MakeTransaction(request.ServiceNumber, request, userId);
                return CreatedAtAction(nameof(GetService), new { id = serviceNumber }, record);
            }
            logger.LogWarning("Could not create service. User unauthenticated");
            return Unauthorized();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<TransactionRecord>> DeleteService(
            [ServiceNumber] string id,
            DeleteServiceRequest request,
            [FromServices] ITransaction<DeleteServiceRequest> transaction)
        {
            logger.LogInformation("DeleteService requested for {serviceNumber}", id);

            if (request.UserId is Guid userId && userManager.IsLoggedIn(userId))
            {
                if (userManager.UserType(userId) != LoginType.Planner)
                {
                    logger.LogWarning("DeleteService requested by non-planner user {userId}", userId);
                    return Forbid();
                }

                var serviceNumber = new ServiceNumber(id);

                if (!servicesList.IsInList(serviceNumber))
                {
                    logger.LogWarning("Could not delete service. No service found with number {serviceNumber}", serviceNumber);
                    return NotFound();
                }

                var record = await transaction.MakeTransaction(id, request, userId);
                servicesList.DeleteService(serviceNumber);

                return Ok(record);
            }
            logger.LogWarning("Could not delete service. User unauthenticated");
            return Unauthorized();
        }

        [HttpPost("{id}/tickets")]
        public async Task<ActionResult<TransactionRecord>> SellOrChangeTickets(
            [ServiceNumber] string id,
            SellOrChangeTicketsRequest request,
            [FromServices] ITransaction<SellTicketsCommand> sellTransaction,
            [FromServices] ITransaction<ChangeTicketsCommand> changeTransaction)
        {
            logger.LogInformation("SellOrChangeTickets requested for {serviceNumber}", id);

            if (request.UserId is Guid userId && userManager.IsLoggedIn(userId))
            {
                var serviceNumber = new ServiceNumber(id);
                TransactionRecord record;

                if (request.SourceServiceNumber is null) // Sell
                {
                    if (!servicesList.IsInList(serviceNumber))
                    {
                        logger.LogWarning("Could not sell or change tickets. No service found with number {serviceNumber}", serviceNumber);
                        return NotFound();
                    }

                    var command = new SellTicketsCommand();
                    command.NumberTickets = int.Parse(request.NumberTickets);

                    record = await sellTransaction.MakeTransaction(id, command, userId);
                }
                else // Change. id is dest number
                {
                    if (!servicesList.IsInList(serviceNumber))
                    {
                        logger.LogWarning("Could not change tickets. No service found with number {destinationServiceNumber}", serviceNumber);
                        return NotFound();
                    }

                    var sourceServiceNumber = new ServiceNumber(request.SourceServiceNumber);
                    if (!servicesList.IsInList(sourceServiceNumber))
                    {
                        logger.LogWarning("Could not change tickets. No service found with number {sourceServiceNumber}", sourceServiceNumber);
                        return NotFound();
                    }

                    var command = new ChangeTicketsCommand();
                    command.SourceServiceNumber = request.SourceServiceNumber;
                    command.NumberTickets = int.Parse(request.NumberTickets);

                    try
                    {
                        record = await changeTransaction.MakeTransaction(id, command, userId);
                    }
                    catch (AgentLimitExceededException e)
                    {
                        return Problem(detail: e.Message, statusCode: Status429TooManyRequests, title: "Limit Exceeded");
                    }
                }

                return Ok(record);
            }
            logger.LogWarning("Could not sell or change tickets. User unauthenticated");
            return Unauthorized();
        }

        [HttpDelete("{id}/tickets")]
        public async Task<ActionResult<TransactionRecord>> CancelTickets(
            [ServiceNumber] string id,
            CancelTicketsRequest request,
            ITransaction<CancelTicketsRequest> transaction)
        {
            logger.LogInformation("CancelTickets requested for {serviceNumber}", id);

            if (request.UserId is Guid userId && userManager.IsLoggedIn(userId))
            {
                var serviceNumber = new ServiceNumber(id);

                if (!servicesList.IsInList(serviceNumber))
                {
                    return NotFound();
                }

                try
                {
                    var record = await transaction.MakeTransaction(id, request, userId);
                    return Ok(record);
                }
                catch (AgentLimitExceededException e)
                {
                    return Problem(detail: e.Message, statusCode: Status429TooManyRequests, title: "Limit Exceeded");
                }
            }
            logger.LogWarning("Could not cancel tickets. User unauthenticated");
            return Unauthorized();
        }
    }
}
