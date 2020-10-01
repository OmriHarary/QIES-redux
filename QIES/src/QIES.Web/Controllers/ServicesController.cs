using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QIES.Api.Models;
using QIES.Api.Models.Validation;
using QIES.Common.Records;
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
        public async Task<IActionResult> GetService([ServiceNumber] string serviceNum)
        {
            var serviceNumber = new ServiceNumber(serviceNum);

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
        /// <response code="201"></response>
        /// <response code="400"></response>
        /// <response code="401"></response>
        /// <response code="403"></response>
        /// <response code="409"></response>
        [HttpPost]
        [ProducesResponseType(Status201Created)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status401Unauthorized)]
        [ProducesResponseType(Status403Forbidden)]
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

                if (servicesList.IsInList(request.ServiceNumber))
                {
                    logger.LogWarning("Could not create service. Requested service number, {serviceNumber}, already allocated", request.ServiceNumber);
                    return Conflict();
                }

                var command = new CreateServiceCommand(request.ServiceNumber, request.ServiceDate, request.ServiceName);
                var record = await transaction.MakeTransaction(command, userId);
                return CreatedAtAction(nameof(GetService), new { id = request.ServiceNumber }, record);
            }
            logger.LogWarning("Could not create service. User unauthenticated");
            return Unauthorized();
        }

        /// <summary>
        /// Delete an existing service.
        /// </summary>
        /// <param name="serviceNum"></param>
        /// <param name="request"></param>
        /// <param name="xUserId"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        /// <response code="201"></response>
        /// <response code="400"></response>
        /// <response code="401"></response>
        /// <response code="403"></response>
        /// <response code="404"></response>
        [HttpDelete("{serviceNum}")]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status401Unauthorized)]
        [ProducesResponseType(Status403Forbidden)]
        [ProducesResponseType(Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<TransactionRecord>> DeleteService(
            [ServiceNumber] string serviceNum,
            DeleteServiceRequest request,
            [FromHeader(Name = "X-User-Id")] Guid? xUserId,
            [FromServices] ITransaction<DeleteServiceCommand> transaction)
        {
            logger.LogInformation("DeleteService requested for {serviceNumber}", serviceNum);

            if (xUserId is Guid userId && userManager.IsLoggedIn(userId))
            {
                if (userManager.UserType(userId) != LoginType.Planner)
                {
                    logger.LogWarning("DeleteService requested by non-planner user {userId}", userId);
                    return Problem(title: "Forbidden", statusCode: Status403Forbidden, detail: "Must be logged in as Planner to delete services.");
                }

                var serviceNumber = new ServiceNumber(serviceNum);

                if (!servicesList.IsInList(serviceNumber))
                {
                    logger.LogWarning("Could not delete service. No service found with number {serviceNumber}", serviceNumber);
                    return NotFound();
                }

                var command = new DeleteServiceCommand(serviceNumber, request.ServiceName);
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
        /// <param name="serviceNum"></param>
        /// <param name="request"></param>
        /// <param name="xUserId"></param>
        /// <param name="sellTransaction"></param>
        /// <param name="changeTransaction"></param>
        /// <returns></returns>
        /// <response code="200"></response>
        /// <response code="400"></response>
        /// <response code="401"></response>
        /// <response code="404"></response>
        /// <response code="429"></response>
        [HttpPost("{serviceNum}/tickets")]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status401Unauthorized)]
        [ProducesResponseType(Status404NotFound)]
        [ProducesResponseType(Status429TooManyRequests)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<TransactionRecord>> SellOrChangeTickets(
            [ServiceNumber] string serviceNum,
            SellOrChangeTicketsRequest request,
            [FromHeader(Name = "X-User-Id")] Guid? xUserId,
            [FromServices] ITransaction<SellTicketsCommand> sellTransaction,
            [FromServices] ITransaction<ChangeTicketsCommand> changeTransaction)
        {
            logger.LogInformation("SellOrChangeTickets requested for {serviceNumber}", serviceNum);

            if (xUserId is Guid userId && userManager.IsLoggedIn(userId))
            {
                var serviceNumber = new ServiceNumber(serviceNum);
                TransactionRecord record;

                if (request.SourceServiceNumber is null) // Sell
                {
                    logger.LogInformation("No source service number in request. Selling tickets for {serviceNumber}", serviceNum);
                    if (!servicesList.IsInList(serviceNumber))
                    {
                        logger.LogWarning("Could not sell or change tickets. No service found with number {serviceNumber}", serviceNumber);
                        return NotFound();
                    }

                    var command = new SellTicketsCommand(serviceNumber, request.NumberTickets);

                    record = await sellTransaction.MakeTransaction(command, userId);
                }
                else // Change. id is dest number
                {
                    logger.LogInformation("Found source service number in request. Changing tickets from {sourceServiceNumber} to {destinationServiceNumber}",
                        request.SourceServiceNumber.Number, serviceNum);

                    if (!servicesList.IsInList(serviceNumber))
                    {
                        logger.LogWarning("Could not change tickets. No service found with number {destinationServiceNumber}", serviceNumber);
                        return NotFound();
                    }

                    var sourceServiceNumber = request.SourceServiceNumber;
                    if (!servicesList.IsInList(sourceServiceNumber))
                    {
                        logger.LogWarning("Could not change tickets. No service found with number {sourceServiceNumber}", sourceServiceNumber);
                        return NotFound();
                    }

                    var command = new ChangeTicketsCommand(serviceNumber, request.NumberTickets, request.SourceServiceNumber);

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
        /// <param name="serviceNum"></param>
        /// <param name="request"></param>
        /// <param name="xUserId"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        /// <response code="200"></response>
        /// <response code="400"></response>
        /// <response code="401"></response>
        /// <response code="404"></response>
        /// <response code="429"></response>
        [HttpDelete("{serviceNum}/tickets")]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status401Unauthorized)]
        [ProducesResponseType(Status404NotFound)]
        [ProducesResponseType(Status429TooManyRequests)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<TransactionRecord>> CancelTickets(
            [ServiceNumber] string serviceNum,
            CancelTicketsRequest request,
            [FromHeader(Name = "X-User-Id")] Guid? xUserId,
            [FromServices] ITransaction<CancelTicketsCommand> transaction)
        {
            logger.LogInformation("CancelTickets requested for {serviceNumber}", serviceNum);

            if (xUserId is Guid userId && userManager.IsLoggedIn(userId))
            {
                var serviceNumber = new ServiceNumber(serviceNum);

                if (!servicesList.IsInList(serviceNumber))
                {
                    return NotFound();
                }

                try
                {
                    var command = new CancelTicketsCommand(serviceNumber, request.NumberTickets);
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
