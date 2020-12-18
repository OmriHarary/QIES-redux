using QIES.Common;
using QIES.Common.Records;

namespace QIES.Backoffice.Parser
{
    public interface ICentralServicesList
    {
        public void Add(Service service);

        public void Delete(ServiceNumber serviceNumber);

        public Service Get(ServiceNumber serviceNumber);

        public bool Contains(ServiceNumber serviceNumber);

        public string[] ValidServicesFileContents();

        public string[] CentralServicesFileContents();
    }
}
