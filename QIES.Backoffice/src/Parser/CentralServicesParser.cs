using System;
using System.IO;
using QIES.Common.Record;

namespace QIES.Backoffice.Parser
{
    public static class CentralServicesParser
    {
        public static CentralServicesList ParseFile(string centralServicesFilePath)
        {
            var centralServices = new CentralServicesList();
            var lines = new string[0];
            try
            {
                lines = File.ReadAllLines(centralServicesFilePath);
            }
            catch (IOException e)
            {
                Console.Error.WriteLine("Unable to read old central services file");
                Console.Error.WriteLine(e.StackTrace);
            }

            foreach (var line in lines)
            {
                centralServices.Add(ParseLine(line));
            }

            return centralServices;
        }

        private static Service ParseLine(string serviceLine)
        {
            var fields = serviceLine.Split(' ', 4);
            var service = new Service();

            service.ServiceNumber = new ServiceNumber(fields[0]);
            service.ServiceCapacity = int.Parse(fields[1]);
            if (fields[2] != "0")
            {
                service.TicketsSold = new NumberTickets(int.Parse(fields[2]));
            }
            service.ServiceName = new ServiceName(fields[3]);

            return service;
        }
    }
}