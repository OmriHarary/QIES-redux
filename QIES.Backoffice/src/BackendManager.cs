using System;
using System.IO;
using QIES.Backoffice.Parser;
using QIES.Backoffice.Parser.Record;

namespace QIES.Backoffice
{
    public class BackendManager
    {
        private TransactionQueue transactionQueue;
        private CentralServicesList centralServicesList;
        string centralServicesOutput;
        string validServicesOutput;

        public BackendManager(string mergedTransactionFile, string oldCentralFile, string newCentralFile, string newValidFile)
        {
            this.transactionQueue = TransactionSummaryParser.ParseFile(mergedTransactionFile);
            this.centralServicesList = CentralServicesParser.ParseFile(oldCentralFile);
            this.centralServicesOutput = newCentralFile;
            this.validServicesOutput = newValidFile;
        }

        public void Operate()
        {
            TransactionRecord record;
            while (!transactionQueue.IsEmpty())
            {
                record = transactionQueue.Pop();

                switch (record.Code)
                {
                    case TransactionCode.CRE:
                        ProcessCRE(record);
                        break;
                    case TransactionCode.DEL:
                        ProcessDEL(record);
                        break;
                    case TransactionCode.SEL:
                        ProcessSEL(record);
                        break;
                    case TransactionCode.CAN:
                        ProcessCAN(record);
                        break;
                    case TransactionCode.CHG:
                        ProcessCHG(record);
                        break;
                    case TransactionCode.EOS:
                        break;
                }
            }

            WriteValidServicesFile();
            WriteCentralServicesFile();
        }

        private void ProcessCRE(TransactionRecord record)
        {
            var log = "Failed to create new service. A service with that number already exists.";
            if (!centralServicesList.Contains(record.SourceNumber))
            {
                var newService = new Service();
                newService.ServiceNumber = record.SourceNumber;
                newService.ServiceName = record.ServiceName;
                centralServicesList.Add(newService);
                log = "New service created";
            }
            Console.WriteLine(log);
        }

        private void ProcessDEL(TransactionRecord record)
        {
            var log = "Failed to delete service. Service does not exist.";
            if (centralServicesList.Contains(record.SourceNumber))
            {
                var toDelete = centralServicesList.Get(record.SourceNumber);
                log = "Failed to delete service. Service still has sold tickets.";
                if (toDelete.TicketsSold.Number == 0)
                {
                    centralServicesList.Delete(toDelete.ServiceNumber);
                    log = "Service deleted";
                }
            }
            Console.WriteLine(log);
        }

        private void ProcessSEL(TransactionRecord record)
        {
            var log = "Failed to sell tickets. Service does not exist";
            if (centralServicesList.Contains(record.SourceNumber))
            {
                var service = centralServicesList.Get(record.SourceNumber);
                try
                {
                    service.AddTickets(record.NumberTickets.Number);
                    log = "Successfully sold tickets.";
                }
                catch (System.ArgumentException)
                {
                    log = "Failed to sell tickets. Number to sell will exceed service capacity.";
                }
            }
            Console.WriteLine(log);
        }

        private void ProcessCAN(TransactionRecord record)
        {
            var log = "Failed to cancel tickets. The service does not exist.";
            if (centralServicesList.Contains(record.SourceNumber))
            {
                var service = centralServicesList.Get(record.SourceNumber);
                try
                {
                    service.RemoveTickets(record.NumberTickets.Number);
                    log = "Successfully cancelled tickets.";
                }
                catch (System.ArgumentException)
                {
                    log = "Failed to cancel tickets. Number to cancel exceeds number of tickets sold.";
                }
            }
            Console.WriteLine(log);
        }

        private void ProcessCHG(TransactionRecord record)
        {
            var log = "Failed to change tickets. Source service does not exist.";
            if (centralServicesList.Contains(record.SourceNumber))
            {
                var source = centralServicesList.Get(record.SourceNumber);
                log = "Failed to change tickets. Destination service does not exist.";
                if (centralServicesList.Contains(record.DestinationNumber))
                {
                    var destination = centralServicesList.Get(record.DestinationNumber);
                    var transfer = record.NumberTickets.Number;
                    log = "Failed to change tickets. Tickets to change exceeds number of tickets sold on source service.";
                    if (transfer <= source.TicketsSold.Number)
                    {
                        log = "Failed to change tickets. Tickets to change will exceed destination service capacity.";
                        if ((destination.TicketsSold.Number + transfer) < destination.ServiceCapacity)
                        {
                            source.RemoveTickets(transfer);
                            destination.AddTickets(transfer);
                            log = "Successfully changed tickets.";
                        }
                    }
                }
            }
            Console.WriteLine(log);
        }

        private void WriteValidServicesFile()
        {
            try
            {
                File.Delete(validServicesOutput);
                File.WriteAllLines(validServicesOutput, centralServicesList.ValidServicesFileContents());
            }
            catch (System.IO.IOException e)
            {
                Console.Error.WriteLine(e.StackTrace);
            }
        }

        private void WriteCentralServicesFile()
        {
            try
            {
                File.Delete(centralServicesOutput);
                File.WriteAllLines(centralServicesOutput, centralServicesList.CentralServicesFileContents());
            }
            catch (System.IO.IOException e)
            {
                Console.Error.WriteLine(e.StackTrace);
            }
        }
    }
}