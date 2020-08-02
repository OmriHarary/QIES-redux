using System.Threading.Tasks;
using QIES.Core.Users;

namespace QIES.Core.Services
{
    public interface ILoginService
    {
        public Task<User> DoLogin(LoginType login);
    }
}
