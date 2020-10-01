using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using QIES.Common.Records;
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
            var sourceServiceNum = "11111";
            var sourceServiceNumber = new ServiceNumber(sourceServiceNum);
            var destinationServiceNum = "11112";
            var destinationServiceNumber = new ServiceNumber(destinationServiceNum);
            var numTickets = 1;
            var numberTickets = new NumberTickets(numTickets);

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
                SourceNumber = sourceServiceNumber,
                DestinationNumber = destinationServiceNumber,
                NumberTickets = numberTickets
            };

            Assert.Equal(expectedRecord, record);
            Assert.Equal(numberTickets.Number, agent.ChangedTickets);
        }


        [Fact]
        public async Task MakeTransaction_AgentServiceLimitExceededSingleAsAgent_AgentLimitExceededExceptionThrown()
        {
            // Arrange
            var sourceServiceNum = "11111";
            var sourceServiceNumber = new ServiceNumber(sourceServiceNum);
            var destinationServiceNum = "11112";
            var destinationServiceNumber = new ServiceNumber(destinationServiceNum);
            var numTickets = 21;
            var numberTickets = new NumberTickets(numTickets);

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
            var sourceServiceNum = "11111";
            var sourceServiceNumber = new ServiceNumber(sourceServiceNum);
            var destinationServiceNum = "11112";
            var destinationServiceNumber = new ServiceNumber(destinationServiceNum);
            var numTickets = 11;
            var numberTickets = new NumberTickets(numTickets);

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
            var sourceServiceNum = "11111";
            var sourceServiceNumber = new ServiceNumber(sourceServiceNum);
            var destinationServiceNum = "11112";
            var destinationServiceNumber = new ServiceNumber(destinationServiceNum);
            var numTickets = 21;
            var numberTickets = new NumberTickets(numTickets);

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
                SourceNumber = sourceServiceNumber,
                DestinationNumber = destinationServiceNumber,
                NumberTickets = numberTickets
            };

            Assert.Equal(expectedRecord, record);
        }

        [Fact]
        public async Task MakeTransaction_SameTransactionRecordPushedAsReturned()
        {
            // Arrange
            var sourceServiceNum = "11111";
            var sourceServiceNumber = new ServiceNumber(sourceServiceNum);
            var destinationServiceNum = "11112";
            var destinationServiceNumber = new ServiceNumber(destinationServiceNum);
            var numTickets = 1;
            var numberTickets = new NumberTickets(numTickets);

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
