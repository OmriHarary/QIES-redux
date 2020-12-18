using System;
using System.IO;
using Microsoft.Extensions.Logging;
using QIES.Backoffice.Parser.Files;
using QIES.Common;
using QIES.Common.Records;

namespace QIES.Backoffice.Parser
{
    public class CentralServicesParser : IParser<CentralServicesList>
    {
        private readonly ILogger<CentralServicesParser> logger;

        public CentralServicesParser(ILogger<CentralServicesParser> logger)
        {
            this.logger = logger;
        }

        public bool TryParseFile(IParserInputFile csFile, CentralServicesList output)
        {
            logger.LogInformation("Attempting to parse central services file at {filePath}", csFile.Path);

            string[] lines;
            try
            {
                lines = csFile.ReadAllLines();
            }
            catch (FileNotFoundException)
            {
                logger.LogWarning("File {filePath} not found. Creating.", csFile.Path);
                csFile.Create();
                return true;
            }
            catch (IOException e)
            {
                logger.LogError(e, "Unable to read central services file at {filePath}", csFile.Path);
                return false;
            }

            foreach (var line in lines)
            {
                try
                {
                    output.Add(ParseLine(line));
                }
                catch (ArgumentException e)
                {
                    logger.LogError(e, "Unparsable line in central services file: [{line}]", line);
                    return false;
                }
            }

            return true;
        }

        private Service ParseLine(string serviceLine)
        {
            logger.LogDebug("Parsing: [{serviceLine}]", serviceLine);

            var fields = serviceLine.Split(' ', 4);
            var service = new Service
            {
                ServiceNumber = new ServiceNumber(fields[0])
            };

            var capacityParsed = int.TryParse(fields[1], out int capacity);
            if (!capacityParsed)
            {
                throw new ArgumentException("Unable to parse capacity.");
            }
            service.ServiceCapacity = capacity;

            if (fields[2] != "0")
            {
                var ticketsParsed = int.TryParse(fields[2], out int tickets);
                if (!ticketsParsed)
                {
                    throw new ArgumentException("Unable to parse tickets.");
                }
                service.TicketsSold = new NumberTickets(tickets);
            }
            service.ServiceName = new ServiceName(fields[3]);

            return service;
        }
    }
}
