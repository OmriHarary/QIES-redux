using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QIES.Backoffice.Config;
using QIES.Backoffice.Parser;
using QIES.Backoffice.Parser.Files;
using QIES.Backoffice.Processor;
using QIES.Common;

namespace QIES.Backoffice
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var exit = 0;

            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch
            {
                exit = 1;
            }

            return exit;
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<TransactionSummaryOptions>(hostContext.Configuration.GetSection(TransactionSummaryOptions.Section));
                    services.Configure<ServicesFilesOptions>(hostContext.Configuration.GetSection(ServicesFilesOptions.Section));
                    services.AddScoped<IParser<TransactionQueue>, TransactionSummaryParser>();
                    services.AddScoped<IParser<CentralServicesList>, CentralServicesParser>();
                    services.AddScoped<IProcessor, BackofficeProcessor>();
                    services.AddSingleton<ICentralServicesList, CentralServicesList>();
                    services.AddHostedService<Worker>();
                });
    }
}
