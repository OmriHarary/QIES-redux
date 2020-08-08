using System.Threading.Tasks;

namespace QIES.Cli.Shell
{
    public interface IShellService
    {
        public Task<int> RunAsync();
    }
}
