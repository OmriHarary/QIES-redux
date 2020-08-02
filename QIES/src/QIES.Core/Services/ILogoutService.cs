using System;
using System.Threading.Tasks;

namespace QIES.Core.Services
{
    public interface ILogoutService
    {
        public Task<bool> DoLogout(Guid id);
    }
}
