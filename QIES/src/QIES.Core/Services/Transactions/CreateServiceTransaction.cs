using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using QIES.Api.Models;
using QIES.Common;
using QIES.Common.Record;
using QIES.Core.Users;

namespace QIES.Core.Services
{
    public class CreateServiceTransaction : ITransaction<CreateServiceRequest, Service>
    {
        private const TransactionCode Code = TransactionCode.CRE;
        private readonly ILogger<CreateServiceTransaction> logger;
        private readonly IUserManager userManager;

        public CreateServiceTransaction(ILogger<CreateServiceTransaction> logger, IUserManager userManager)
        {
            this.logger = logger;
            this.userManager = userManager;
        }

        public async Task<Service> MakeTransaction(string serviceNumber, CreateServiceRequest request, Guid userId)
        {
            var service = new Service();
            service.ServiceNumber = new ServiceNumber(request.ServiceNumber);
            service.ServiceName = new ServiceName(request.ServiceName);
            // service.ServiceDate = new ServiceDate(request.ServiceDate);

            var record = new TransactionRecord(Code);
            record.SourceNumber = new ServiceNumber(request.ServiceNumber);
            record.ServiceDate = new ServiceDate(request.ServiceDate);
            record.ServiceName = new ServiceName(request.ServiceName);

            userManager.UserTransactionQueue(userId).Push(record);

            return service;
        }
    }
}
