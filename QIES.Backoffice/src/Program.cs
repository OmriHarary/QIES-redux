using System;

namespace QIES.Backoffice
{
    public class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.Error.WriteLine($"Incorrect number of arguments: {args.Length}");
                return 1;
            }

            string mergedTransactionFile = args[0];
            string oldCentralFile = args[1];
            string newCentralFile = args[2];
            string newValidFile = args[3];
            BackendManager backendManager = new BackendManager(mergedTransactionFile, oldCentralFile, newCentralFile, newValidFile);
            backendManager.Operate();

            return 0;
        }
    }
}
