using System.Collections.Generic;
using QIES.Common.Record;

namespace QIES.Core.Users
{
    public class Agent : User
    {
        public int ChangedTickets { get; set; }
        private int totalCancelledTickets;
        private readonly Dictionary<ServiceNumber, int> cancelledTickets;

        public Agent() : base(LoginType.Agent)
        {
            this.cancelledTickets = new Dictionary<ServiceNumber, int>();
        }
    }
}
