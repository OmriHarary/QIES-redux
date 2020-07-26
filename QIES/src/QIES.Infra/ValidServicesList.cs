using System.Collections.Concurrent;
using System.IO;
using QIES.Common.Record;
using QIES.Core;

namespace QIES.Infra
{
    public class ValidServicesList : IServicesList
    {
        private ConcurrentDictionary<string, byte> validServices;

        public ValidServicesList(FileInfo validServicesFile)
        {
            validServices = new ConcurrentDictionary<string, byte>();
            ReadServices(validServicesFile);
        }

        public ValidServicesList()
        {
            validServices = new ConcurrentDictionary<string, byte>();
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
                        validServices.TryAdd((new ServiceNumber(line)).Number, 0);
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
            return validServices.Keys.Contains(service);
        }

        public void DeleteService(string service)
        {
            validServices.TryRemove(service, out _);
        }

        public void AddService(string service)
        {
            validServices.TryAdd(service, 0);
        }
    }
}
