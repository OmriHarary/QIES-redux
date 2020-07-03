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

            var mergedTransactionFile = args[0];
            var oldCentralFile = args[1];
            var newCentralFile = args[2];
            var newValidFile = args[3];
            var backendManager = new BackendManager(mergedTransactionFile, oldCentralFile, newCentralFile, newValidFile);
            backendManager.Operate();

            return 0;
        }
    }
}
