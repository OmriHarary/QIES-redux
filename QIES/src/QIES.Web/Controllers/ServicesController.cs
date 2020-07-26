using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QIES.Api.Models;
using QIES.Api.Models.Validation;
using QIES.Common;
using QIES.Common.Record;
using QIES.Core;
using QIES.Core.Services;

using static System.Net.Mime.MediaTypeNames;

namespace QIES.Web.Controllers
{
    [ApiController]
    [Consumes(Application.Json)]
    [Produces(Application.Json)]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
    {
        private IServicesList servicesList;
        private ITransaction<CreateServiceRequest, Service> createServiceTransaction;
        private ITransaction<DeleteServiceRequest, TransactionRecord> deleteServiceTransaction;
        private ITransaction<SellOrChangeTicketsRequest, TransactionRecord> sellOrChangeTicketsTransaction;
        private ITransaction<CancelTicketsRequest, TransactionRecord> cancelTicketsTransaction;

        public ServicesController(
                IServicesList servicesList,
                ITransaction<CreateServiceRequest, Service> createServiceTransaction,
                ITransaction<DeleteServiceRequest, TransactionRecord> deleteServiceTransaction,
                ITransaction<SellOrChangeTicketsRequest, TransactionRecord> sellOrChangeTicketsTransaction,
                ITransaction<CancelTicketsRequest, TransactionRecord> cancelTicketsTransaction)
        {
            this.servicesList = servicesList;
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
            if (servicesList.IsInList(id))
            {
                return Ok();
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<Service>> CreateService(CreateServiceRequest createServiceRequest)
        {
            if (servicesList.IsInList(createServiceRequest.ServiceNumber))
            {
                return Conflict();
            }

            var service = await createServiceTransaction.MakeTransaction(createServiceRequest.ServiceNumber, createServiceRequest);
            return CreatedAtAction(nameof(GetService), new { id = service.ServiceNumber }, service);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<TransactionRecord>> DeleteService([ServiceNumber] string id, DeleteServiceRequest request)
        {
            if (!servicesList.IsInList(id))
            {
                return NotFound();
            }

            var record = await deleteServiceTransaction.MakeTransaction(id, request);
            servicesList.DeleteService(id);

            return Ok(record);
        }

        [HttpPost("{id}/tickets")]
        public async Task<IActionResult> SellOrChangeTickets([ServiceNumber] string id, SellOrChangeTicketsRequest request)
        {
            if (!servicesList.IsInList(id))
            {
                return NotFound();
            }

            if (request.SourceServiceNumber is not null && !servicesList.IsInList(request.SourceServiceNumber))
            {
                return NotFound();
            }

            var record = await sellOrChangeTicketsTransaction.MakeTransaction(id, request);

            return Ok(record);
        }

        [HttpDelete("{id}/tickets")]
        public async Task<IActionResult> CancelTickets([ServiceNumber] string id, CancelTicketsRequest request)
        {
            if (!servicesList.IsInList(id))
            {
                return NotFound();
            }

            var record = await cancelTicketsTransaction.MakeTransaction(id, request);

            return Ok(record);
        }
    }
}
