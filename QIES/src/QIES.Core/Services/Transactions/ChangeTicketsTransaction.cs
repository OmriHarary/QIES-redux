using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using QIES.Common.Record;
using QIES.Core.Commands;
using QIES.Core.Users;

namespace QIES.Core.Services
{
    public class ChangeTicketsTransaction : ITransaction<ChangeTicketsCommand>
    {
        private const TransactionCode Code = TransactionCode.CHG;
        private readonly ILogger<ChangeTicketsTransaction> logger;
        private readonly IUserManager userManager;

        public ChangeTicketsTransaction(ILogger<ChangeTicketsTransaction> logger, IUserManager userManager)
        {
            this.logger = logger;
            this.userManager = userManager;
        }

        public async Task<TransactionRecord> MakeTransaction(string serviceNumber, ChangeTicketsCommand command, Guid userId)
        {
            var record = new TransactionRecord(Code)
            {
                SourceNumber = new ServiceNumber(command.SourceServiceNumber),
                DestinationNumber = new ServiceNumber(serviceNumber),
                NumberTickets = new NumberTickets(command.NumberTickets)
            };

            if (userManager.UserType(userId) == LoginType.Agent && userManager.User(userId) is Agent agent)
            {
                if (agent.ChangedTickets + command.NumberTickets > 20)
                {
                    throw new AgentLimitExceededException("Cannot change as total session changed tickets would be over 20." +
                        $" User has {20 - agent.ChangedTickets} tickets left to change this session.");
                }
                agent.ChangedTickets += command.NumberTickets;
            }

            userManager.UserTransactionQueue(userId).Push(record);

            return record;
        }
    }
}
