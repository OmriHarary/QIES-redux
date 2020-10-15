using System.Collections.Concurrent;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QIES.Common.Records;
using QIES.Core;
using QIES.Core.Config;

namespace QIES.Infra
{
    public class ValidServicesList : IServicesList
    {
        private readonly ILogger<ValidServicesList> logger;
        private readonly ConcurrentDictionary<ServiceNumber, byte> validServices;

        public ValidServicesList(IOptions<ValidServicesListOptions> options, ILogger<ValidServicesList> logger)
        {
            this.logger = logger;
            validServices = new ConcurrentDictionary<ServiceNumber, byte>();
            ReadServices(new FileInfo(options.Value.Path));
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
                        AddService(new ServiceNumber(line));
                    }
                }
            }
            catch (IOException e)
            {
                // TODO: Actual error handling (the original didn't handle this either)
                logger.LogError(e.StackTrace);
            }
        }

        public bool IsInList(ServiceNumber service) => validServices.Keys.Contains(service);

        public void DeleteService(ServiceNumber service) => validServices.TryRemove(service, out _);

        public void AddService(ServiceNumber service) => validServices.TryAdd(service, 0);
    }
}
