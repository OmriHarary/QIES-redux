using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QIES.Cli.Client;
using QIES.Cli.Shell;

namespace QIES.Cli
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine($"Incorrect number of arguments: {args.Length}");
                return 1;
            }

            var builder = new HostBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.AddDebug();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHttpClient<QIESClient>(c =>
                    {
                        c.BaseAddress = new Uri(args[0]);
                    });
                    services.AddTransient<IShellService, ShellService>();
                }).UseConsoleLifetime();

            var host = builder.Build();
            int exit;

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                try
                {
                    var shell = services.GetRequiredService<IShellService>();
                    exit = await shell.RunAsync();
                }
                catch (Exception)
                {
                    exit = 1;
                }
            }

            return exit;
        }
    }
}
