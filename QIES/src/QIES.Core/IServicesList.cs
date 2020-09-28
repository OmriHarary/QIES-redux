using QIES.Common.Records;

namespace QIES.Core
{
    public interface IServicesList
    {
        public void AddService(ServiceNumber service);

        public void DeleteService(ServiceNumber service);

        public bool IsInList(ServiceNumber service);
    }
}
