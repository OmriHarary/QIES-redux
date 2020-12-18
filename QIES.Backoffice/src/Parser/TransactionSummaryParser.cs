using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using QIES.Backoffice.Parser.Files;
using QIES.Common;
using QIES.Common.Records;

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
                    if (TransactionRecord.TryParse(line, out var transaction))
                    {
                        output.Push(transaction!); // Can remove bang after #64 is resolved
                        successful++;
                    }
                    else
                    {
                        logger.LogWarning("Failed to parse transaction: [{line}] It will be skipped.", line);
                    }
                }
            }

            logger.LogInformation("Succesfully parsed {successful}/{total} records from file.", successful, count);
            return true;
        }
    }
}
