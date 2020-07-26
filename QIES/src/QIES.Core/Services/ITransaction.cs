using System.Threading.Tasks;

namespace QIES.Core.Services
{
    public interface ITransaction<TRequest, TResponse>
    {
        public Task<TResponse> MakeTransaction(string serviceNumber, TRequest request);
    }
}
