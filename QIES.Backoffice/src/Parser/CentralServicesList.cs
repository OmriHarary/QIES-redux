using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using QIES.Common;
using QIES.Common.Record;

namespace QIES.Backoffice.Parser
{
    public class CentralServicesList : ICentralServicesList
    {
        private IDictionary<ServiceNumber, Service> services;

        public CentralServicesList() => this.services = new ConcurrentDictionary<ServiceNumber, Service>();

        public void Add(Service service) => services.Add(service.ServiceNumber, service);

        public void Delete(ServiceNumber serviceNumber) => services.Remove(serviceNumber);

        public Service Get(ServiceNumber serviceNumber) => services[serviceNumber];

        public bool Contains(ServiceNumber serviceNumber) => services.ContainsKey(serviceNumber);

        public string[] ValidServicesFileContents()
        {
            var lines = new ServiceNumber[services.Count];
            services.Keys.CopyTo(lines, 0);
            var strings = Array.ConvertAll(lines, l => l.ToString());
            Array.Sort(strings);
            var vsl = new string[services.Count + 1];
            strings.CopyTo(vsl, 0);
            vsl[^1] = "00000";
            return vsl;
        }

        public string[] CentralServicesFileContents()
        {
            var lines = new Service[services.Count];
            services.Values.CopyTo(lines, 0);
            var csl = Array.ConvertAll(lines, l => l.ToString());
            Array.Sort(csl);
            return csl;
        }
    }
}
