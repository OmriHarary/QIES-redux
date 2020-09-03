using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QIES.Api.Models;
using QIES.Api.Models.Validation;
using QIES.Common;
using QIES.Common.Record;
using QIES.Core;
using QIES.Core.Services;
using QIES.Core.Users;
using static System.Net.Mime.MediaTypeNames;

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
        private ITransaction<CreateServiceRequest, Service> createServiceTransaction;
        private ITransaction<DeleteServiceRequest, TransactionRecord> deleteServiceTransaction;
        private ITransaction<SellOrChangeTicketsRequest, TransactionRecord> sellOrChangeTicketsTransaction;
        private ITransaction<CancelTicketsRequest, TransactionRecord> cancelTicketsTransaction;

        public ServicesController(
                ILogger<ServicesController> logger,
                IServicesList servicesList,
                IUserManager userManager,
                ITransaction<CreateServiceRequest, Service> createServiceTransaction,
                ITransaction<DeleteServiceRequest, TransactionRecord> deleteServiceTransaction,
                ITransaction<SellOrChangeTicketsRequest, TransactionRecord> sellOrChangeTicketsTransaction,
                ITransaction<CancelTicketsRequest, TransactionRecord> cancelTicketsTransaction)
        {
            this.logger = logger;
            this.servicesList = servicesList;
            this.userManager = userManager;
            this.createServiceTransaction = createServiceTransaction;
            this.deleteServiceTransaction = deleteServiceTransaction;
            this.sellOrChangeTicketsTransaction = sellOrChangeTicketsTransaction;
            this.cancelTicketsTransaction = cancelTicketsTransaction;
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
        public async Task<ActionResult<Service>> CreateService(CreateServiceRequest request)
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

                var service = await createServiceTransaction.MakeTransaction(request.ServiceNumber, request, userId);
                return CreatedAtAction(nameof(GetService), new { id = service.ServiceNumber }, service);
            }
            logger.LogWarning("Could not create service. User unauthenticated");
            return Unauthorized();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<TransactionRecord>> DeleteService([ServiceNumber] string id, DeleteServiceRequest request)
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

                var record = await deleteServiceTransaction.MakeTransaction(id, request, userId);
                servicesList.DeleteService(serviceNumber);

                return Ok(record);
            }
            logger.LogWarning("Could not delete service. User unauthenticated");
            return Unauthorized();
        }

        [HttpPost("{id}/tickets")]
        public async Task<ActionResult<TransactionRecord>> SellOrChangeTickets([ServiceNumber] string id, SellOrChangeTicketsRequest request)
        {
            logger.LogInformation("SellOrChangeTickets requested for {serviceNumber}", id);

            if (request.UserId is Guid userId && userManager.IsLoggedIn(userId))
            {
                var serviceNumber = new ServiceNumber(id);

                if (!servicesList.IsInList(serviceNumber))
                {
                    logger.LogWarning("Could not sell or change tickets. No service found with number {serviceNumber}", serviceNumber);
                    return NotFound();
                }

                if (request.SourceServiceNumber is not null)
                {
                    var sourceServiceNumber = new ServiceNumber(request.SourceServiceNumber);
                    if (!servicesList.IsInList(sourceServiceNumber))
                    {
                        logger.LogWarning("Could not change tickets. No service found with number {sourceServiceNumber}", sourceServiceNumber);
                        return NotFound();
                    }
                }

                var record = await sellOrChangeTicketsTransaction.MakeTransaction(id, request, userId);

                return Ok(record);
            }
            logger.LogWarning("Could not sell or change tickets. User unauthenticated");
            return Unauthorized();
        }

        [HttpDelete("{id}/tickets")]
        public async Task<ActionResult<TransactionRecord>> CancelTickets([ServiceNumber] string id, CancelTicketsRequest request)
        {
            logger.LogInformation("CancelTickets requested for {serviceNumber}", id);

            if (request.UserId is Guid userId && userManager.IsLoggedIn(userId))
            {
                var serviceNumber = new ServiceNumber(id);

                if (!servicesList.IsInList(serviceNumber))
                {
                    return NotFound();
                }

                var record = await cancelTicketsTransaction.MakeTransaction(id, request, userId);

                return Ok(record);
            }
            logger.LogWarning("Could not cancel tickets. User unauthenticated");
            return Unauthorized();
        }
    }
}
