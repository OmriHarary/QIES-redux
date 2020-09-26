using System;
using System.Threading.Tasks;
using QIES.Common.Record;
using QIES.Core.Commands;

namespace QIES.Core.Services
{
    public interface ITransaction<T> where T : TransactionCommand
    {
        public Task<TransactionRecord> MakeTransaction(T command, Guid userId);
    }
}
