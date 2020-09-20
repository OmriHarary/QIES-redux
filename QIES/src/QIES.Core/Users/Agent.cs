using System.Collections.Generic;
using QIES.Common.Record;

namespace QIES.Core.Users
{
    public class Agent : User
    {
        public int ChangedTickets { get; set; }
        public int TotalCancelledTickets { get; set; }
        public Dictionary<ServiceNumber, int> CancelledTickets { get; private set; }

        public Agent() : base(LoginType.Agent)
        {
            this.CancelledTickets = new Dictionary<ServiceNumber, int>();
        }
    }
}
