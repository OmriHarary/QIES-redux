using System;
using QIES.Cli.Session;
using QIES.Frontend.Session;

namespace QIES.Cli
{
    public class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.Error.WriteLine($"Incorrect number of arguments: {args.Length}");
                return 1;
            }

            var validServicesFilePath = args[0];
            var summaryFilePath = args[1];
            var sessionController = new SessionController(validServicesFilePath, summaryFilePath);
            var sessionClient = new SessionClient(sessionController);

            return sessionClient.Operate();;
        }
    }
}
