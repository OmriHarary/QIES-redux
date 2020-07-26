using System.Threading.Tasks;
using QIES.Api.Models;
using QIES.Common;
using QIES.Common.Record;

namespace QIES.Core.Services
{
    public class CreateServiceTransaction : ITransaction<CreateServiceRequest, Service>
    {
        private const TransactionCode Code = TransactionCode.CRE;
        private ITransactionQueue transactionQueue;

        public CreateServiceTransaction(ITransactionQueue transactionQueue)
        {
            this.transactionQueue = transactionQueue;
        }

        public async Task<Service> MakeTransaction(string serviceNumber, CreateServiceRequest request)
        {
            var service = new Service();
            service.ServiceNumber = new ServiceNumber(request.ServiceNumber);
            service.ServiceName = new ServiceName(request.ServiceName);
            // service.ServiceDate = new ServiceDate(request.ServiceDate);

            var record = new TransactionRecord(Code);
            record.SourceNumber = new ServiceNumber(request.ServiceNumber);
            record.ServiceDate = new ServiceDate(request.ServiceDate);
            record.ServiceName = new ServiceName(request.ServiceName);

            transactionQueue.Push(record);

            return service;
        }
    }
}
