using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using QIES.Backoffice.Config;
using QIES.Backoffice.Parser;
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
                    logging.AddJsonConsole();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOpenTelemetry()
                        .ConfigureResource(resourceBuilder => resourceBuilder
                            .AddService(
                                serviceName: hostContext.HostingEnvironment.ApplicationName,
                                serviceVersion: Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown"))
                        .WithTracing(builder => builder
                            .AddOtlpExporter())
                        .WithMetrics(builder => builder
                            .AddRuntimeInstrumentation()
                            .AddPrometheusHttpListener(
                                options => options.UriPrefixes = new string[] { "http://localhost:5001/" }))
                        .StartWithHost();

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
