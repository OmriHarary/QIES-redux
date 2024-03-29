using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QIES.Backoffice.Config;
using QIES.Backoffice.Parser;
using QIES.Backoffice.Parser.Files;
using QIES.Common;
using QIES.Common.Records;

namespace QIES.Backoffice.Processor
{
    public class BackofficeProcessor : IProcessor
    {
        private readonly ILogger<BackofficeProcessor> logger;
        private readonly ServicesFilesOptions outputPaths;
        private readonly IParser<TransactionQueue> transactionSummaryParser;
        private readonly ICentralServicesList centralServices;

        public BackofficeProcessor(
            ILogger<BackofficeProcessor> logger,
            IOptions<ServicesFilesOptions> options,
            IParser<TransactionQueue> transactionSummaryParser,
            ICentralServicesList centralServices)
        {
            this.logger = logger;
            this.outputPaths = options.Value;
            this.transactionSummaryParser = transactionSummaryParser;
            this.centralServices = centralServices;
        }

        public void Process(FileInfo transactionFile)
        {
            using var processActivity = Program.BackofficeActivitySource.StartActivity(transactionFile.Name);

            using (logger.BeginScope(transactionFile.Name))
            {
                logger.LogInformation("Processing {transactionFile}", transactionFile.FullName);

                var transactionQueue = new TransactionQueue();
                using (logger.BeginScope("Parse"))
                {
                    if (!transactionSummaryParser.TryParseFile(new ParserInputFile(transactionFile.FullName), transactionQueue))
                    {
                        logger.LogError("Skipping unreadable transaction summary file.");
                        processActivity?.SetStatus(ActivityStatusCode.Error, "Transaction summary file unreadable.");
                        return;
                    }
                }

                var count = 0;
                var successful = 0;
                TransactionRecord record;
                while (!transactionQueue.IsEmpty())
                {
                    record = transactionQueue.Pop();
                    count++;

                    using (logger.BeginScope(record.ToString()))
                    {
                        var processed = record.Code switch
                        {
                            TransactionCode.CRE => ProcessCRE(record),
                            TransactionCode.DEL => ProcessDEL(record),
                            TransactionCode.SEL => ProcessSEL(record),
                            TransactionCode.CAN => ProcessCAN(record),
                            TransactionCode.CHG => ProcessCHG(record),
                            TransactionCode.EOS => true,
                            _                   => false
                        };

                        if (processed)
                        {
                            successful++;
                        }
                    }
                }

                logger.LogInformation("Successfully processed {successful}/{total} parsed records.", successful, count);

                // NOTE: Not inlined in the following if so that csf failure doesn't skip vsl by short circuiting.
                //          Reminder for next time I think about refactoring this.
                var csf = WriteCentralServicesFile();
                var vsl = WriteValidServicesFile();

                if (csf && vsl)
                {
                    logger.LogInformation("Files updated. Processing complete.");
                }
                else
                {
                    logger.LogError("One or more output files failed to update.");
                }
            }
        }

        public bool ProcessCRE(TransactionRecord record)
        {
            using var processCREActivity = Program.BackofficeActivitySource.StartActivity(nameof(ProcessCRE));

            if (centralServices.Contains(record.SourceNumber))
            {
                logger.LogWarning("Failed to create new service. A service with that number already exists.");
                return false;
            }

            var newService = new Service
            {
                ServiceNumber = record.SourceNumber,
                ServiceName = record.ServiceName
            };
            centralServices.Add(newService);
            logger.LogInformation("New service created");
            return true;
        }

        public bool ProcessDEL(TransactionRecord record)
        {
            using var processDELActivity = Program.BackofficeActivitySource.StartActivity(nameof(ProcessDEL));

            if (!centralServices.Contains(record.SourceNumber))
            {
                logger.LogWarning("Failed to delete service. Service does not exist.");
                return false;
            }

            var toDelete = centralServices.Get(record.SourceNumber);
            if (toDelete.TicketsSold.Number != 0)
            {
                logger.LogWarning("Failed to delete service. Service still has sold tickets.");
                return false;
            }

            centralServices.Delete(toDelete.ServiceNumber);
            logger.LogInformation("Service deleted");
            return true;
        }

        public bool ProcessSEL(TransactionRecord record)
        {
            using var processSELActivity = Program.BackofficeActivitySource.StartActivity(nameof(ProcessSEL));

            if (!centralServices.Contains(record.SourceNumber))
            {
                logger.LogWarning("Failed to sell tickets. Service does not exist");
                return false;
            }

            var service = centralServices.Get(record.SourceNumber);
            try
            {
                service.AddTickets(record.NumberTickets.Number);
                logger.LogInformation("Successfully sold tickets.");
                return true;
            }
            catch (System.ArgumentException)
            {
                logger.LogWarning("Failed to sell tickets. Number to sell will exceed service capacity.");
                return false;
            }
        }

        public bool ProcessCAN(TransactionRecord record)
        {
            using var processCANActivity = Program.BackofficeActivitySource.StartActivity(nameof(ProcessCAN));

            if (!centralServices.Contains(record.SourceNumber))
            {
                logger.LogWarning("Failed to cancel tickets. The service does not exist.");
                return false;
            }

            var service = centralServices.Get(record.SourceNumber);
            try
            {
                service.RemoveTickets(record.NumberTickets.Number);
                logger.LogInformation("Successfully cancelled tickets.");
                return true;
            }
            catch (System.ArgumentException)
            {
                logger.LogWarning("Failed to cancel tickets. Number to cancel exceeds number of tickets sold.");
                return false;
            }
        }

        public bool ProcessCHG(TransactionRecord record)
        {
            using var processCHGActivity = Program.BackofficeActivitySource.StartActivity(nameof(ProcessCHG));

            if (!centralServices.Contains(record.SourceNumber))
            {
                logger.LogWarning("Failed to change tickets. Source service does not exist.");
                return false;
            }
            var source = centralServices.Get(record.SourceNumber);

            if (!centralServices.Contains(record.DestinationNumber))
            {
                logger.LogWarning("Failed to change tickets. Destination service does not exist.");
                return false;
            }
            var destination = centralServices.Get(record.DestinationNumber);

            var transfer = record.NumberTickets.Number;
            if (transfer > source.TicketsSold.Number)
            {
                logger.LogWarning("Failed to change tickets. Tickets to change exceeds number of tickets sold on source service.");
                return false;
            }
            if ((destination.TicketsSold.Number + transfer) > destination.ServiceCapacity)
            {
                logger.LogWarning("Failed to change tickets. Tickets to change will exceed destination service capacity.");
                return false;
            }

            source.RemoveTickets(transfer);
            destination.AddTickets(transfer);
            logger.LogInformation("Successfully changed tickets.");
            return true;
        }

        private bool WriteValidServicesFile()
        {
            try
            {
                File.Delete(outputPaths.ValidServicesFile);
                File.WriteAllLines(outputPaths.ValidServicesFile, centralServices.ValidServicesFileContents());
            }
            catch (System.Exception e)
            {
                logger.LogError(e, "Error writing valid services file.");
                return false;
            }
            return true;
        }

        private bool WriteCentralServicesFile()
        {
            try
            {
                File.Delete(outputPaths.CentralServicesFile);
                File.WriteAllLines(outputPaths.CentralServicesFile, centralServices.CentralServicesFileContents());
            }
            catch (System.Exception e)
            {
                logger.LogError(e, "Error writing central services file.");
                return false;
            }
            return true;
        }
    }
}
