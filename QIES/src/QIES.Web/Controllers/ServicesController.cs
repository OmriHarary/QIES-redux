using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QIES.Api.Models;
using QIES.Api.Models.Validation;
using QIES.Common;
using QIES.Core;
using QIES.Core.Services;

using static System.Net.Mime.MediaTypeNames;

namespace QIES.Web.Controllers
{
    [ApiController]
    [Consumes(Application.Json)]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
    {
        private IServicesList servicesList;
        private ITransaction<CreateServiceRequest, Service> createServiceTransaction;

        public ServicesController(
                IServicesList servicesList,
                ITransaction<CreateServiceRequest, Service> createServiceTransaction)
        {
            this.servicesList = servicesList;
            this.createServiceTransaction = createServiceTransaction;
        }


        // [HttpGet]
        // public async Task<ActionResult<IEnumerable<Service>>> GetServices()
        // {
        //     return new List<Service>();
        // }

        [HttpGet("{id}")]
        public async Task<ActionResult<Service>> GetService(string id)
        {
            return null!;
        }

        [HttpPost]
        public async Task<ActionResult<Service>> CreateService(CreateServiceRequest createServiceRequest)
        {
            if (servicesList.IsInList(createServiceRequest.ServiceNumber))
            {
                return Conflict();
            }

            var service = await createServiceTransaction.MakeTransaction(createServiceRequest);
            return CreatedAtAction(nameof(GetService), new { id = service.ServiceNumber}, service);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService([ServiceNumber] string id)
        {
            if (!servicesList.IsInList(id))
            {
                return NotFound();
            }
            servicesList.DeleteService(id);
            return NoContent();
        }

    }
}
