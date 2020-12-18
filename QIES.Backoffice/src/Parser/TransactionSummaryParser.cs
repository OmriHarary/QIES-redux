using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using QIES.Backoffice.Parser.Files;
using QIES.Common;
using QIES.Common.Record;

namespace QIES.Backoffice.Parser
{
    public class TransactionSummaryParser : IParser<TransactionQueue>
    {
        private readonly ILogger<TransactionSummaryParser> logger;

        public TransactionSummaryParser(ILogger<TransactionSummaryParser> logger)
        {
            this.logger = logger;
        }

        public bool TryParseFile(IParserInputFile tsFile, TransactionQueue output)
        {
            string[] lines;
            try
            {
                lines = tsFile.ReadAllLines();
            }
            catch (IOException e)
            {
                logger.LogError(e, "Unable to read transaction summary file at {filePath}", tsFile.Path);
                return false;
            }

            var count = 0;
            var successful = 0;
            foreach (var line in lines)
            {
                count++;
                using (logger.BeginScope(line))
                {
                    try
                    {
                        var transaction = ParseLine(line);
                        output.Push(transaction);
                        successful++;
                    }
                    catch (Exception e)
                    {
                        logger.LogWarning(e, "Failed to parse transaction: [{line}] It will be skipped.", line);
                    }
                }
            }

            logger.LogInformation("Succesfully parsed {successful}/{total} records from file.", successful, count);
            return true;
        }

        private TransactionRecord ParseLine(string transactionLine)
        {
            var tokens = transactionLine.Split(' ');
            var code = (TransactionCode)Enum.Parse(typeof(TransactionCode), tokens[0]);
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

            logger.LogDebug("Parsed");
            return record;
        }
    }
}
