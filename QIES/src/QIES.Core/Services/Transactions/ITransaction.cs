using System;
using System.Threading.Tasks;
using QIES.Common.Record;

namespace QIES.Core.Services
{
    public interface ITransaction<T>
    {
        public Task<TransactionRecord> MakeTransaction(string serviceNumber, T request, Guid userId);
    }
}
