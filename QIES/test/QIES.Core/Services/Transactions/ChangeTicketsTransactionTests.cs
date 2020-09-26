using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using QIES.Common.Record;
using QIES.Core.Commands;
using QIES.Core.Users;
using Xunit;

namespace QIES.Core.Services.Tests
{
    public class ChangeTicketsTransactionTests
    {
        [Fact]
        public async Task MakeTransaction_AsAgent_CorrectTransactionRecordCreated()
        {
            // Arrange
            var sourceServiceNumber = "11111";
            var destinationServiceNumber = "11112";
            var numberTickets = 1;

            var logger = new Mock<ILogger<ChangeTicketsTransaction>>();
            var userManager = new Mock<IUserManager>();
            var transactionQueue = new Mock<ITransactionQueue>();
            var agent = new Agent();

            userManager.Setup(userManager => userManager.UserTransactionQueue(It.IsAny<Guid>()))
                .Returns(transactionQueue.Object);
            userManager.Setup(userManager => userManager.UserType(It.IsAny<Guid>()))
                .Returns(LoginType.Agent);
            userManager.Setup(userManager => userManager.User(It.IsAny<Guid>()))
                .Returns(agent);

            var command = new ChangeTicketsCommand(destinationServiceNumber, numberTickets, sourceServiceNumber);

            var transaction = new ChangeTicketsTransaction(logger.Object, userManager.Object);

            // Act
            var record = await transaction.MakeTransaction(command, Guid.NewGuid());

            // Assert
            var expectedRecord = new TransactionRecord(TransactionCode.CHG)
            {
                SourceNumber = new ServiceNumber(sourceServiceNumber),
                DestinationNumber = new ServiceNumber(destinationServiceNumber),
                NumberTickets = new NumberTickets(numberTickets)
            };

            Assert.Equal(expectedRecord, record);
            Assert.Equal(numberTickets, agent.ChangedTickets);
        }


        [Fact]
        public async Task MakeTransaction_AgentServiceLimitExceededSingleAsAgent_AgentLimitExceededExceptionThrown()
        {
            // Arrange
            var sourceServiceNumber = "11111";
            var destinationServiceNumber = "11112";
            var numberTickets = 21;

            var logger = new Mock<ILogger<ChangeTicketsTransaction>>();
            var userManager = new Mock<IUserManager>();
            var transactionQueue = new Mock<ITransactionQueue>();
            var agent = new Agent();

            userManager.Setup(userManager => userManager.UserTransactionQueue(It.IsAny<Guid>()))
                .Returns(transactionQueue.Object);
            userManager.Setup(userManager => userManager.UserType(It.IsAny<Guid>()))
                .Returns(LoginType.Agent);
            userManager.Setup(userManager => userManager.User(It.IsAny<Guid>()))
                .Returns(agent);

            var command = new ChangeTicketsCommand(destinationServiceNumber, numberTickets, sourceServiceNumber);

            var transaction = new ChangeTicketsTransaction(logger.Object, userManager.Object);

            // Act & Assert
            await Assert.ThrowsAsync<AgentLimitExceededException>(() => transaction.MakeTransaction(command, Guid.NewGuid()));
        }

        [Fact]
        public async Task MakeTransaction_AgentServiceLimitExceededMultipleAsAgent_AgentLimitExceededExceptionThrown()
        {
            // Arrange
            var sourceServiceNumber = "11111";
            var destinationServiceNumber = "11112";
            var numberTickets = 11;

            var logger = new Mock<ILogger<ChangeTicketsTransaction>>();
            var userManager = new Mock<IUserManager>();
            var transactionQueue = new Mock<ITransactionQueue>();
            var agent = new Agent();

            userManager.Setup(userManager => userManager.UserTransactionQueue(It.IsAny<Guid>()))
                .Returns(transactionQueue.Object);
            userManager.Setup(userManager => userManager.UserType(It.IsAny<Guid>()))
                .Returns(LoginType.Agent);
            userManager.Setup(userManager => userManager.User(It.IsAny<Guid>()))
                .Returns(agent);

            var command = new ChangeTicketsCommand(destinationServiceNumber, numberTickets, sourceServiceNumber);

            agent.ChangedTickets = 10;
            var transaction = new ChangeTicketsTransaction(logger.Object, userManager.Object);

            // Act & Assert
            await Assert.ThrowsAsync<AgentLimitExceededException>(() => transaction.MakeTransaction(command, Guid.NewGuid()));
        }

        [Fact]
        public async Task MakeTransaction_AgentServiceLimitExceededSingleAsPlanner_NoExceptionThrown()
        {
            // Arrange
            var sourceServiceNumber = "11111";
            var destinationServiceNumber = "11112";
            var numberTickets = 21;

            var logger = new Mock<ILogger<ChangeTicketsTransaction>>();
            var userManager = new Mock<IUserManager>();
            var transactionQueue = new Mock<ITransactionQueue>();
            var planner = new Planner();

            userManager.Setup(userManager => userManager.UserTransactionQueue(It.IsAny<Guid>()))
                .Returns(transactionQueue.Object);
            userManager.Setup(userManager => userManager.UserType(It.IsAny<Guid>()))
                .Returns(LoginType.Planner);
            userManager.Setup(userManager => userManager.User(It.IsAny<Guid>()))
                .Returns(planner);

            var command = new ChangeTicketsCommand(destinationServiceNumber, numberTickets, sourceServiceNumber);

            var transaction = new ChangeTicketsTransaction(logger.Object, userManager.Object);

            // Act
            var record = await transaction.MakeTransaction(command, Guid.NewGuid());

            // Assert
            var expectedRecord = new TransactionRecord(TransactionCode.CHG)
            {
                SourceNumber = new ServiceNumber(sourceServiceNumber),
                DestinationNumber = new ServiceNumber(destinationServiceNumber),
                NumberTickets = new NumberTickets(numberTickets)
            };

            Assert.Equal(expectedRecord, record);
        }

        [Fact]
        public async Task MakeTransaction_SameTransactionRecordPushedAsReturned()
        {
            // Arrange
            var sourceServiceNumber = "11111";
            var destinationServiceNumber = "11112";
            var numberTickets = 1;

            var logger = new Mock<ILogger<ChangeTicketsTransaction>>();
            var userManager = new Mock<IUserManager>();
            var transactionQueue = new Mock<ITransactionQueue>();
            userManager.Setup(userManager => userManager.UserTransactionQueue(It.IsAny<Guid>()))
                .Returns(transactionQueue.Object);

            var command = new ChangeTicketsCommand(destinationServiceNumber, numberTickets, sourceServiceNumber);

            var transaction = new ChangeTicketsTransaction(logger.Object, userManager.Object);

            // Act
            var record = await transaction.MakeTransaction(command, Guid.NewGuid());

            // Assert
            transactionQueue.Verify(transactionQueue => transactionQueue.Push(record));
        }
    }
}
