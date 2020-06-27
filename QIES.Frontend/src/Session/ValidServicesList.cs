using System.Collections.Generic;
using System.IO;
using QIES.Frontend.Transaction.Record;

namespace QIES.Frontend.Session
{
    class ValidServicesList
    {
        private HashSet<ServiceNumber> validServices;

        public ValidServicesList(FileInfo validServicesFile)
        {

        }

        private void ReadServices(FileInfo validServicesFile)
        {

        }

        public bool IsInList(ServiceNumber service)
        {
            return false;
        }
    }
}
