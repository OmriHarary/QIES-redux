using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QIES.Backoffice.Config;
using QIES.Backoffice.Parser;
using QIES.Backoffice.Processor;

namespace QIES.Backoffice
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly ICentralServicesList centralServices;
        private readonly ServicesFilesOptions servicesFilesOptions;
        private readonly TransactionSummaryOptions transactionSummaryOptions;
        private FileSystemWatcher summaryFileWatcher;

        public IServiceProvider Services { get; }

        public Worker(
                ILogger<Worker> logger,
                ICentralServicesList centralServices,
                IOptions<ServicesFilesOptions> sfOptions,
                IOptions<TransactionSummaryOptions> tsOptions,
                IServiceProvider services)
        {
            this.logger = logger;
            this.centralServices = centralServices;
            this.servicesFilesOptions = sfOptions.Value;
            this.transactionSummaryOptions = tsOptions.Value;
            this.Services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting worker service");
            if (!Directory.Exists(transactionSummaryOptions.Directory))
            {
                var ex = new DirectoryNotFoundException();
                logger.LogCritical(ex, $"Specified input directory {transactionSummaryOptions.Directory} does not exist.");
                return Task.FromException(ex);
            }

            using (var scope = Services.CreateScope())
            {
                var centralServicesParser = scope.ServiceProvider.GetRequiredService<IParser<CentralServicesList>>();
                var parsed = centralServicesParser.TryParseFile(servicesFilesOptions.CentralServicesFile, (CentralServicesList)centralServices);
                if (!parsed)
                {
                    logger.LogCritical("Central services file parsing failed.");
                    return Task.FromException(new System.Exception()); // TODO: Replace with a specific exception
                }
            }

            summaryFileWatcher = new FileSystemWatcher(transactionSummaryOptions.Directory, transactionSummaryOptions.Filter)
            {
                NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName
            };

            summaryFileWatcher.Created += OnFileCreated;
            summaryFileWatcher.EnableRaisingEvents = true;

            return base.StartAsync(cancellationToken);
        }

        protected void OnFileCreated(object source, FileSystemEventArgs args)
        {
            if (args.ChangeType == WatcherChangeTypes.Created)
            {
                logger.LogDebug($"File watcher event triggered on {args.Name}");
                var file = new FileInfo(args.FullPath);

                using (var scope = Services.CreateScope())
                {
                    var processor = scope.ServiceProvider.GetRequiredService<IProcessor>();
                    processor.Process(file);
                }

                file.Delete();
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopping worker service");
            summaryFileWatcher.EnableRaisingEvents = false;
            // TODO: Flush CSL here?

            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            summaryFileWatcher?.Dispose();
            base.Dispose();
        }
    }
}
