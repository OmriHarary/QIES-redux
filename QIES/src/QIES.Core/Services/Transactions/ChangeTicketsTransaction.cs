using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using QIES.Common.Records;
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

        public async Task<TransactionRecord> MakeTransaction(ChangeTicketsCommand command, Guid userId)
        {
            var record = new TransactionRecord(Code)
            {
                SourceNumber = command.SourceServiceNumber,
                DestinationNumber = command.ServiceNumber,
                NumberTickets = command.NumberTickets
            };

            if (userManager.UserType(userId) == LoginType.Agent && userManager.User(userId) is Agent agent)
            {
                if (agent.ChangedTickets + record.NumberTickets.Number > 20)
                {
                    throw new AgentLimitExceededException("Cannot change as total session changed tickets would be over 20." +
                        $" User has {20 - agent.ChangedTickets} tickets left to change this session.");
                }
                agent.ChangedTickets += record.NumberTickets.Number;
            }

            logger.LogDebug("Pushing record to queue: {transaction}", record);
            userManager.UserTransactionQueue(userId).Push(record);

            return record;
        }
    }
}
