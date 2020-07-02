using System.Collections.Generic;
using System.IO;
using QIES.Frontend.Transaction.Record;

namespace QIES.Frontend.Session
{
    public class ValidServicesList
    {
        private HashSet<string> validServices;

        public ValidServicesList(FileInfo validServicesFile)
        {
            validServices = new HashSet<string>();
            ReadServices(validServicesFile);
        }

        private void ReadServices(FileInfo validServicesFile)
        {
            try
            {
                using StreamReader validServicesReader = validServicesFile.OpenText();
                string line;
                while ((line = validServicesReader.ReadLine()) != null)
                {
                    if (line != "00000")
                    {
                        validServices.Add((new ServiceNumber(line)).Number);
                    }
                }
            }
            catch (IOException e)
            {
                // TODO: Actual error handling (the original didn't handle this either)
                System.Console.Error.WriteLine(e.StackTrace);
            }

        }

        public bool IsInList(string service)
        {
            return validServices.Contains(service);
        }

        public void DeleteService(string service)
        {
            validServices.Remove(service);
        }
    }
}
