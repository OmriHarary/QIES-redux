using System.Collections.Generic;
using System.IO;
using QIES.Common.Record;
using QIES.Core;

namespace QIES.Infra
{
    public class ValidServicesList : IServicesList
    {
        private HashSet<string> validServices;

        public ValidServicesList(FileInfo validServicesFile)
        {
            validServices = new HashSet<string>();
            ReadServices(validServicesFile);
        }

        public ValidServicesList()
        {
            validServices = new HashSet<string>();
        }

        private void ReadServices(FileInfo validServicesFile)
        {
            try
            {
                using StreamReader validServicesReader = validServicesFile.OpenText();
                string? line;
                while (!((line = validServicesReader.ReadLine()) is null))
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

        public void AddService(string service)
        {
            validServices.Add(service);
        }
    }
}
