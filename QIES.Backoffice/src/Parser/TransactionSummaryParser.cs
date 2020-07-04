using System;
using System.IO;
using QIES.Common;
using QIES.Common.Record;

namespace QIES.Backoffice.Parser
{
    public static class TransactionSummaryParser
    {
        public static TransactionQueue ParseFile(string transactionSummaryFilePath)
        {
            var transactionQueue = new TransactionQueue();
            var lines = new string[0];
            try
            {
                lines = File.ReadAllLines(transactionSummaryFilePath);
            }
            catch (IOException e)
            {
                Console.Error.WriteLine("Unable to read merged transaction summary file");
                Console.Error.WriteLine(e.StackTrace);
            }

            foreach (var line in lines)
            {
                transactionQueue.Push(ParseLine(line));
            }

            return transactionQueue;
        }

        private static TransactionRecord ParseLine(string transactionLine)
        {
            var tokens = transactionLine.Split(' ');
            var code = (TransactionCode) Enum.Parse(typeof(TransactionCode), tokens[0]);
            var record = new TransactionRecord(code);
            // TODO: Make RecordElement Defaults public and use them here for comparisons
            if (tokens[1] != "00000")
            {
                record.SourceNumber = new ServiceNumber(tokens[1]);
            }
            if (tokens[2] != "0")
            {
                record.NumberTickets = new NumberTickets(int.Parse(tokens[2]));
            }
            if (tokens[3] != "00000")
            {
                record.DestinationNumber = new ServiceNumber(tokens[3]);
            }
            if (tokens[^1] != "0")
            {
                record.ServiceDate = new ServiceDate(tokens[^1]);
            }
            var serviceNameStr = string.Join(' ', tokens[4..^1]);
            if (serviceNameStr != "****")
            {
                record.ServiceName = new ServiceName(serviceNameStr);
            }

            return record;
        }
    }
}