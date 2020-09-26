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
    public class CancelTicketsTransactionTests
    {
        [Fact]
        public async Task MakeTransaction_AsAgent_CorrectTransactionRecordCreated()
        {
            // Arrange
            var serviceNum = "11111";
            var serviceNumber = new ServiceNumber(serviceNum);
            var numTickets = 1;

            var logger = new Mock<ILogger<CancelTicketsTransaction>>();
            var userManager = new Mock<IUserManager>();
            var transactionQueue = new Mock<ITransactionQueue>();
            var agent = new Agent();

            userManager.Setup(userManager => userManager.UserTransactionQueue(It.IsAny<Guid>()))
                .Returns(transactionQueue.Object);
            userManager.Setup(userManager => userManager.UserType(It.IsAny<Guid>()))
                .Returns(LoginType.Agent);
            userManager.Setup(userManager => userManager.User(It.IsAny<Guid>()))
                .Returns(agent);

            var command = new CancelTicketsCommand(serviceNum, numTickets);

            var transaction = new CancelTicketsTransaction(logger.Object, userManager.Object);

            // Act
            var record = await transaction.MakeTransaction(command, Guid.NewGuid());

            // Assert
            var expectedRecord = new TransactionRecord(TransactionCode.CAN)
            {
                SourceNumber = serviceNumber,
                NumberTickets = new NumberTickets(numTickets)
            };

            Assert.Equal(expectedRecord, record);
            Assert.Equal(numTickets, agent.CancelledTickets[serviceNumber]);
            Assert.Equal(numTickets, agent.TotalCancelledTickets);
        }

        [Fact]
        public async Task MakeTransaction_AgentServiceLimitExceededSingleAsAgent_AgentLimitExceededExceptionThrown()
        {
            // Arrange
            var serviceNum = "11111";
            var serviceNumber = new ServiceNumber(serviceNum);
            var numTickets = 11;

            var logger = new Mock<ILogger<CancelTicketsTransaction>>();
            var userManager = new Mock<IUserManager>();
            var transactionQueue = new Mock<ITransactionQueue>();
            var agent = new Agent();

            userManager.Setup(userManager => userManager.UserTransactionQueue(It.IsAny<Guid>()))
                .Returns(transactionQueue.Object);
            userManager.Setup(userManager => userManager.UserType(It.IsAny<Guid>()))
                .Returns(LoginType.Agent);
            userManager.Setup(userManager => userManager.User(It.IsAny<Guid>()))
                .Returns(agent);

            var command = new CancelTicketsCommand(serviceNum, numTickets);

            var transaction = new CancelTicketsTransaction(logger.Object, userManager.Object);

            // Act & Assert
            await Assert.ThrowsAsync<AgentLimitExceededException>(() => transaction.MakeTransaction(command, Guid.NewGuid()));
        }

        [Fact]
        public async Task MakeTransaction_AgentServiceLimitExceededMultipleAsAgent_AgentLimitExceededExceptionThrown()
        {
            // Arrange
            var serviceNum = "11111";
            var serviceNumber = new ServiceNumber(serviceNum);
            var numTickets = 3;

            var logger = new Mock<ILogger<CancelTicketsTransaction>>();
            var userManager = new Mock<IUserManager>();
            var transactionQueue = new Mock<ITransactionQueue>();
            var agent = new Agent();

            userManager.Setup(userManager => userManager.UserTransactionQueue(It.IsAny<Guid>()))
                .Returns(transactionQueue.Object);
            userManager.Setup(userManager => userManager.UserType(It.IsAny<Guid>()))
                .Returns(LoginType.Agent);
            userManager.Setup(userManager => userManager.User(It.IsAny<Guid>()))
                .Returns(agent);

            var command = new CancelTicketsCommand(serviceNum, numTickets);

            agent.CancelledTickets.Add(serviceNumber, 8);
            var transaction = new CancelTicketsTransaction(logger.Object, userManager.Object);

            // Act & Assert
            await Assert.ThrowsAsync<AgentLimitExceededException>(() => transaction.MakeTransaction(command, Guid.NewGuid()));
        }

        [Fact]
        public async Task MakeTransaction_AgentSessionLimitExceededAsAgent_AgentLimitExceededExceptionThrown()
        {
            // Arrange
            var serviceNum = "11111";
            var serviceNumber = new ServiceNumber(serviceNum);
            var numTickets = 5;

            var logger = new Mock<ILogger<CancelTicketsTransaction>>();
            var userManager = new Mock<IUserManager>();
            var transactionQueue = new Mock<ITransactionQueue>();
            var agent = new Agent();

            userManager.Setup(userManager => userManager.UserTransactionQueue(It.IsAny<Guid>()))
                .Returns(transactionQueue.Object);
            userManager.Setup(userManager => userManager.UserType(It.IsAny<Guid>()))
                .Returns(LoginType.Agent);
            userManager.Setup(userManager => userManager.User(It.IsAny<Guid>()))
                .Returns(agent);

            var command = new CancelTicketsCommand(serviceNum, numTickets);

            agent.TotalCancelledTickets = 16;
            var transaction = new CancelTicketsTransaction(logger.Object, userManager.Object);

            // Act & Assert
            await Assert.ThrowsAsync<AgentLimitExceededException>(() => transaction.MakeTransaction(command, Guid.NewGuid()));
        }

        [Fact]
        public async Task MakeTransaction_AgentServiceLimitExceededAsPlanner_NoExceptionThrown()
        {
            // Arrange
            var serviceNum = "11111";
            var serviceNumber = new ServiceNumber(serviceNum);
            var numTickets = 11;

            var logger = new Mock<ILogger<CancelTicketsTransaction>>();
            var userManager = new Mock<IUserManager>();
            var transactionQueue = new Mock<ITransactionQueue>();
            var planner = new Planner();

            userManager.Setup(userManager => userManager.UserTransactionQueue(It.IsAny<Guid>()))
                .Returns(transactionQueue.Object);
            userManager.Setup(userManager => userManager.UserType(It.IsAny<Guid>()))
                .Returns(LoginType.Planner);
            userManager.Setup(userManager => userManager.User(It.IsAny<Guid>()))
                .Returns(planner);

            var command = new CancelTicketsCommand(serviceNum, numTickets);

            var transaction = new CancelTicketsTransaction(logger.Object, userManager.Object);

            // Act
            var record = await transaction.MakeTransaction(command, Guid.NewGuid());

            // Assert
            var expectedRecord = new TransactionRecord(TransactionCode.CAN)
            {
                SourceNumber = serviceNumber,
                NumberTickets = new NumberTickets(numTickets)
            };

            Assert.Equal(expectedRecord, record);
        }

        [Fact]
        public async Task MakeTransaction_AgentSessionLimitExceededAsPlanner_NoExceptionThrown()
        {
            // Arrange
            var serviceNum = "11111";
            var serviceNumber = new ServiceNumber(serviceNum);
            int numTickets = 21;

            var logger = new Mock<ILogger<CancelTicketsTransaction>>();
            var userManager = new Mock<IUserManager>();
            var transactionQueue = new Mock<ITransactionQueue>();
            var planner = new Planner();

            userManager.Setup(userManager => userManager.UserTransactionQueue(It.IsAny<Guid>()))
                .Returns(transactionQueue.Object);
            userManager.Setup(userManager => userManager.UserType(It.IsAny<Guid>()))
                .Returns(LoginType.Planner);
            userManager.Setup(userManager => userManager.User(It.IsAny<Guid>()))
                .Returns(planner);

            var command = new CancelTicketsCommand(serviceNum, numTickets);

            var transaction = new CancelTicketsTransaction(logger.Object, userManager.Object);

            // Act
            var record = await transaction.MakeTransaction(command, Guid.NewGuid());

            // Assert
            var expectedRecord = new TransactionRecord(TransactionCode.CAN)
            {
                SourceNumber = serviceNumber,
                NumberTickets = new NumberTickets(numTickets)
            };

            Assert.Equal(expectedRecord, record);
        }

        [Fact]
        public async Task MakeTransaction_SameTransactionRecordPushedAsReturned()
        {
            // Arrange
            var serviceNum = "11111";
            var serviceNumber = new ServiceNumber(serviceNum);
            var numTickets = 1;

            var logger = new Mock<ILogger<CancelTicketsTransaction>>();
            var userManager = new Mock<IUserManager>();
            var transactionQueue = new Mock<ITransactionQueue>();

            userManager.Setup(userManager => userManager.UserTransactionQueue(It.IsAny<Guid>()))
                .Returns(transactionQueue.Object);

            var command = new CancelTicketsCommand(serviceNum, numTickets);

            var transaction = new CancelTicketsTransaction(logger.Object, userManager.Object);

            // Act
            var record = await transaction.MakeTransaction(command, Guid.NewGuid());

            // Assert
            transactionQueue.Verify(transactionQueue => transactionQueue.Push(record));
        }
    }
}
