using System;
using System.IO;
using QIES.Backoffice.Parser.Record;

namespace QIES.Backoffice.Parser
{
    public static class CentralServicesParser
    {
        public static CentralServicesList ParseFile(string centralServicesFilePath)
        {
            CentralServicesList centralServices = new CentralServicesList();
            string[] lines = new string[0];
            try
            {
                lines = File.ReadAllLines(centralServicesFilePath);
            }
            catch (IOException e)
            {
                Console.Error.WriteLine("Unable to read old central services file");
                Console.Error.WriteLine(e.StackTrace);
            }

            foreach (string line in lines)
            {
                centralServices.Add(ParseLine(line));
            }

            return centralServices;
        }

        private static Service ParseLine(string serviceLine)
        {
            string[] fields = serviceLine.Split(' ', 4);
            Service service = new Service();

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