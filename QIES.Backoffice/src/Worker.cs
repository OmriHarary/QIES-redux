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
    public sealed class Worker : BackgroundService
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
            servicesFilesOptions = sfOptions.Value;
            transactionSummaryOptions = tsOptions.Value;
            Services = services;
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
                logger.LogCritical(ex, "Specified transaction summary input directory {directory} does not exist.", transactionSummaryOptions.Directory);
                return Task.FromException(ex);
            }

            // Create empty valid services file if not already present, for Web component to pick up.
            if (!File.Exists(servicesFilesOptions.ValidServicesFile))
            {
                logger.LogWarning("File {filePath} not found. Creating.", servicesFilesOptions.ValidServicesFile);
                File.CreateText(servicesFilesOptions.ValidServicesFile);
            }

            using (var scope = Services.CreateScope())
            {
                var centralServicesParser = scope.ServiceProvider.GetRequiredService<IParser<CentralServicesList>>();
                var parsed = centralServicesParser.TryParseFile(servicesFilesOptions.CentralServicesFile, (CentralServicesList)centralServices);
                if (!parsed)
                {
                    logger.LogCritical("Central services file parsing failed.");
                    return Task.FromException(new IOException());
                }
            }

            summaryFileWatcher = new FileSystemWatcher(transactionSummaryOptions.Directory, transactionSummaryOptions.Filter)
            {
                NotifyFilter = NotifyFilters.CreationTime
                               | NotifyFilters.LastWrite
                               | NotifyFilters.FileName
                               | NotifyFilters.DirectoryName
            };

            summaryFileWatcher.Created += OnFileCreated;
            summaryFileWatcher.EnableRaisingEvents = true;

            return base.StartAsync(cancellationToken);
        }

        protected void OnFileCreated(object source, FileSystemEventArgs args)
        {
            if (args.ChangeType == WatcherChangeTypes.Created)
            {
                logger.LogDebug("File watcher event triggered on {triggeringFile}", args.Name);
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
