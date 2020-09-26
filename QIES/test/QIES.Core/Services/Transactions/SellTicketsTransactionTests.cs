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
    public class SellTicketsTransactionTests
    {
        [Fact]
        public async Task MakeTransaction_CorrectTransactionRecordCreated()
        {
            // Arrange
            var serviceNumber = "11111";
            var numberTickets = 2;

            var logger = new Mock<ILogger<SellTicketsTransaction>>();
            var userManager = new Mock<IUserManager>();
            var transactionQueue = new Mock<ITransactionQueue>();
            userManager.Setup(userManager => userManager.UserTransactionQueue(It.IsAny<Guid>()))
                .Returns(transactionQueue.Object);

            var command = new SellTicketsCommand(serviceNumber, numberTickets);

            var transaction = new SellTicketsTransaction(logger.Object, userManager.Object);

            // Act
            var record = await transaction.MakeTransaction(command, Guid.NewGuid());

            // Assert
            var expectedRecord = new TransactionRecord(TransactionCode.SEL)
            {
                SourceNumber = new ServiceNumber(serviceNumber),
                NumberTickets = new NumberTickets(numberTickets)
            };

            Assert.Equal(expectedRecord, record);
        }

        [Fact]
        public async Task MakeTransaction_SameTransactionRecordPushedAsReturned()
        {
            // Arrange
            var serviceNumber = "11111";
            var numberTickets = 2;

            var logger = new Mock<ILogger<SellTicketsTransaction>>();
            var userManager = new Mock<IUserManager>();
            var transactionQueue = new Mock<ITransactionQueue>();
            userManager.Setup(userManager => userManager.UserTransactionQueue(It.IsAny<Guid>()))
                .Returns(transactionQueue.Object);

            var command = new SellTicketsCommand(serviceNumber, numberTickets);

            var transaction = new SellTicketsTransaction(logger.Object, userManager.Object);

            // Act
            var record = await transaction.MakeTransaction(command, Guid.NewGuid());

            // Assert
            transactionQueue.Verify(transactionQueue => transactionQueue.Push(record));
        }
    }
}
