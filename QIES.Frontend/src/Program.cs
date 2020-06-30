using System;
using QIES.Frontend.Session;

namespace QIES.Frontend
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

            string validServicesFilePath = args[0];
            string summaryFilePath = args[1];
            SessionManager sessionManager = new SessionManager(validServicesFilePath, summaryFilePath);
            sessionManager.Operate();

            return 0;
        }
    }
}
