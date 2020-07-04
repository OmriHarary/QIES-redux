using System;
using System.Collections.Generic;
using QIES.Backoffice.Parser;
using QIES.Common.Record;

namespace QIES.Backoffice
{
    public class CentralServicesList
    {
        private IDictionary<string, Service> services;

        public CentralServicesList() => this.services = new SortedDictionary<string, Service>();

        public void Add(Service service) => services.Add(service.ServiceNumber.Number, service);

        public void Delete(ServiceNumber serviceNumber) => Delete(serviceNumber.Number);

        private void Delete(string serviceNumber) => services.Remove(serviceNumber);

        public Service Get(ServiceNumber serviceNumber) => Get(serviceNumber.Number);

        private Service Get(string serviceNumber) => services[serviceNumber];

        public bool Contains(ServiceNumber serviceNumber) => Contains(serviceNumber.Number);

        private bool Contains(string serviceNumber) => services.ContainsKey(serviceNumber);

        public string[] ValidServicesFileContents()
        {
            var lines = new string[services.Count + 1];
            services.Keys.CopyTo(lines, 0);
            lines[^1] = "00000";
            return lines;
        }

        public string[] CentralServicesFileContents()
        {
            var lines = new Service[services.Count];
            services.Values.CopyTo(lines, 0);
            return Array.ConvertAll(lines, l => l.ToString());
        }
    }
}