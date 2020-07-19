namespace QIES.Core
{
    public interface IServicesList
    {
        public void AddService(string service);

        public void DeleteService(string service);

        public bool IsInList(string service);
    }
}
