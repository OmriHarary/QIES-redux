using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using QIES.Api.Models;
using QIES.Common.Record;
using QIES.Core.Users;

namespace QIES.Core.Services
{
    public class CancelTicketsTransaction : ITransaction<CancelTicketsRequest>
    {
        private const TransactionCode Code = TransactionCode.CAN;
        private readonly ILogger<CancelTicketsTransaction> logger;
        private readonly IUserManager userManager;

        public CancelTicketsTransaction(ILogger<CancelTicketsTransaction> logger, IUserManager userManager)
        {
            this.logger = logger;
            this.userManager = userManager;
        }

        public async Task<TransactionRecord> MakeTransaction(string serviceNumber, CancelTicketsRequest request, Guid userId)
        {
            var record = new TransactionRecord(Code)
            {
                SourceNumber = new ServiceNumber(serviceNumber),
                NumberTickets = new NumberTickets(int.Parse(request.NumberTickets))
            };

            if (userManager.UserType(userId) == LoginType.Agent && userManager.User(userId) is Agent agent)
            {
                if (!agent.CancelledTickets.ContainsKey(record.SourceNumber))
                {
                    agent.CancelledTickets.Add(record.SourceNumber, 0);
                }
                if (agent.CancelledTickets[record.SourceNumber] + record.NumberTickets.Number > 10)
                {
                    throw new AgentLimitExceededException("Cannot cancel more then 10 tickets for a single service." +
                        $" User has {10 - agent.CancelledTickets[record.SourceNumber]} tickets left to cancel for this service.");
                }
                if (agent.TotalCancelledTickets + record.NumberTickets.Number > 20)
                {
                    throw new AgentLimitExceededException("Cannot cancel as total session canceled tickets would be over 20." +
                        $" User has {20 - agent.TotalCancelledTickets} tickets left to cancel this session.");
                }

                agent.CancelledTickets[record.SourceNumber] += record.NumberTickets.Number;
                agent.TotalCancelledTickets += record.NumberTickets.Number;
            }

            userManager.UserTransactionQueue(userId).Push(record);

            return record;
        }
    }
}
