using System.Threading.Tasks;

namespace QIES.Cli.Shell
{
    public interface IShellService
    {
        public Task<int> RunAsync();
        public Task<(bool, string)> Login();
        public Task<(bool, string)> Logout();
        public Task<(bool, string)> SellTickets();
        public Task<(bool, string)> CancelTickets();
        public Task<(bool, string)> ChangeTickets();
        public Task<(bool, string)> CreateService();
        public Task<(bool, string)> DeleteService();
        public (bool, string) Exit();
    }
}
