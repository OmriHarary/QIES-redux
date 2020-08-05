using System;
using System.IO;
using Microsoft.Extensions.Logging;
using QIES.Common;
using QIES.Common.Record;

namespace QIES.Backoffice.Parser
{
    public class CentralServicesParser : IParser<CentralServicesList>
    {
        private readonly ILogger<CentralServicesParser> logger;

        public CentralServicesParser(ILogger<CentralServicesParser> logger)
        {
            this.logger = logger;
        }

        public bool TryParseFile(string filePath, CentralServicesList output)
        {
            logger.LogInformation($"Attempting to parse central services file at {filePath}");

            var success = true;
            var lines = new string[0];
            try
            {
                lines = File.ReadAllLines(filePath);
            }
            catch (IOException e)
            {
                logger.LogError(e, $"Unable to read central services file at {filePath}");
                success = false;
            }

            foreach (var line in lines)
            {
                try
                {
                    output.Add(ParseLine(line));
                }
                catch (ArgumentException e)
                {
                    logger.LogError(e, $"Unparsable line in central services file: [{line}]");
                    success = false;
                    break;
                }
            }

            return success;
        }

        private Service ParseLine(string serviceLine)
        {
            logger.LogDebug($"Parsing: [{serviceLine}]");

            var fields = serviceLine.Split(' ', 4);
            var service = new Service();

            service.ServiceNumber = new ServiceNumber(fields[0]);

            int capacity;
            var capacityParsed = int.TryParse(fields[1], out capacity);
            if (!capacityParsed)
            {
                throw new ArgumentException();
            }
            service.ServiceCapacity = capacity;

            if (fields[2] != "0")
            {
                int tickets;
                var ticketsParsed = int.TryParse(fields[2], out tickets);
                if (!ticketsParsed)
                {
                    throw new ArgumentException();
                }
                service.TicketsSold = new NumberTickets(tickets);
            }
            service.ServiceName = new ServiceName(fields[3]);

            return service;
        }
    }
}
