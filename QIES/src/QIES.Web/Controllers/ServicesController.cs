using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QIES.Api.Models;
using QIES.Api.Models.Validation;
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
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetService([ServiceNumber] string id)
        {
            var serviceNumber = new ServiceNumber(id);

            if (servicesList.IsInList(serviceNumber))
            {
                return Ok();
            }

            return NotFound();
        }

        /// <summary>
        /// Create a new service.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="xUserId"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(Status201Created)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status401Unauthorized)]
        [ProducesResponseType(Status403Forbidden)]
        [ProducesResponseType(Status404NotFound)]
        [ProducesResponseType(Status409Conflict)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<TransactionRecord>> CreateService(
            CreateServiceRequest request,
            [FromHeader(Name = "X-User-Id")] Guid? xUserId,
            [FromServices] ITransaction<CreateServiceCommand> transaction)
        {
            logger.LogInformation("CreateService requested for {serviceNumber}", request.ServiceNumber);

            if (xUserId is Guid userId && userManager.IsLoggedIn(userId))
            {
                if (userManager.UserType(userId) != LoginType.Planner)
                {
                    logger.LogWarning("CreateService requested by non-planner user {userId}", userId);
                    return Problem(title: "Forbidden", statusCode: Status403Forbidden, detail: "Must be logged in as Planner to create services.");
                }

                var serviceNumber = new ServiceNumber(request.ServiceNumber);

                if (servicesList.IsInList(serviceNumber))
                {
                    logger.LogWarning("Could not create service. Requested service number, {serviceNumber}, already allocated", serviceNumber);
                    return Conflict();
                }

                var command = new CreateServiceCommand(request.ServiceNumber, request.ServiceDate, request.ServiceName);
                var record = await transaction.MakeTransaction(command, userId);
                return CreatedAtAction(nameof(GetService), new { id = serviceNumber }, record);
            }
            logger.LogWarning("Could not create service. User unauthenticated");
            return Unauthorized();
        }

        /// <summary>
        /// Delete an existing service.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <param name="xUserId"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status401Unauthorized)]
        [ProducesResponseType(Status403Forbidden)]
        [ProducesResponseType(Status404NotFound)]
        [ProducesResponseType(Status409Conflict)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<TransactionRecord>> DeleteService(
            [ServiceNumber] string id,
            DeleteServiceRequest request,
            [FromHeader(Name = "X-User-Id")] Guid? xUserId,
            [FromServices] ITransaction<DeleteServiceCommand> transaction)
        {
            logger.LogInformation("DeleteService requested for {serviceNumber}", id);

            if (xUserId is Guid userId && userManager.IsLoggedIn(userId))
            {
                if (userManager.UserType(userId) != LoginType.Planner)
                {
                    logger.LogWarning("DeleteService requested by non-planner user {userId}", userId);
                    return Problem(title: "Forbidden", statusCode: Status403Forbidden, detail: "Must be logged in as Planner to delete services.");
                }

                var serviceNumber = new ServiceNumber(id);

                if (!servicesList.IsInList(serviceNumber))
                {
                    logger.LogWarning("Could not delete service. No service found with number {serviceNumber}", serviceNumber);
                    return NotFound();
                }

                var command = new DeleteServiceCommand(id, request.ServiceName);
                var record = await transaction.MakeTransaction(command, userId);
                servicesList.DeleteService(serviceNumber);

                return Ok(record);
            }
            logger.LogWarning("Could not delete service. User unauthenticated");
            return Unauthorized();
        }

        /// <summary>
        /// Sell tickets for an existing service, or change tickets from one service to another.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <param name="xUserId"></param>
        /// <param name="sellTransaction"></param>
        /// <param name="changeTransaction"></param>
        /// <returns></returns>
        [HttpPost("{id}/tickets")]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status401Unauthorized)]
        [ProducesResponseType(Status404NotFound)]
        [ProducesResponseType(Status429TooManyRequests)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<TransactionRecord>> SellOrChangeTickets(
            [ServiceNumber] string id,
            SellOrChangeTicketsRequest request,
            [FromHeader(Name = "X-User-Id")] Guid? xUserId,
            [FromServices] ITransaction<SellTicketsCommand> sellTransaction,
            [FromServices] ITransaction<ChangeTicketsCommand> changeTransaction)
        {
            logger.LogInformation("SellOrChangeTickets requested for {serviceNumber}", id);

            if (xUserId is Guid userId && userManager.IsLoggedIn(userId))
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

                    var command = new SellTicketsCommand(id, int.Parse(request.NumberTickets));

                    record = await sellTransaction.MakeTransaction(command, userId);
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

                    var command = new ChangeTicketsCommand(id, int.Parse(request.NumberTickets), request.SourceServiceNumber);

                    try
                    {
                        record = await changeTransaction.MakeTransaction(command, userId);
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

        /// <summary>
        /// Cancel sold tickets.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <param name="xUserId"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        [HttpDelete("{id}/tickets")]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status401Unauthorized)]
        [ProducesResponseType(Status404NotFound)]
        [ProducesResponseType(Status429TooManyRequests)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<TransactionRecord>> CancelTickets(
            [ServiceNumber] string id,
            CancelTicketsRequest request,
            [FromHeader(Name = "X-User-Id")] Guid? xUserId,
            [FromServices] ITransaction<CancelTicketsCommand> transaction)
        {
            logger.LogInformation("CancelTickets requested for {serviceNumber}", id);

            if (xUserId is Guid userId && userManager.IsLoggedIn(userId))
            {
                var serviceNumber = new ServiceNumber(id);

                if (!servicesList.IsInList(serviceNumber))
                {
                    return NotFound();
                }

                try
                {
                    var command = new CancelTicketsCommand(id, int.Parse(request.NumberTickets));
                    var record = await transaction.MakeTransaction(command, userId);
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
