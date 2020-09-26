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
    public class CreateServiceTransactionTests
    {
        [Fact]
        public async Task MakeTransaction_CorrectTransactionRecordCreated()
        {
            // Arrange
            var serviceNumber = "11211";
            var serviceName = "ANewService";
            var serviceDate = "20181001";

            var logger = new Mock<ILogger<CreateServiceTransaction>>();
            var userManager = new Mock<IUserManager>();
            var transactionQueue = new Mock<ITransactionQueue>();
            userManager.Setup(userManager => userManager.UserTransactionQueue(It.IsAny<Guid>()))
                .Returns(transactionQueue.Object);

            var command = new CreateServiceCommand(serviceNumber, serviceDate, serviceName);

            var transaction = new CreateServiceTransaction(logger.Object, userManager.Object);

            // Act
            var record = await transaction.MakeTransaction(command, Guid.NewGuid());

            // Assert
            var expectedRecord = new TransactionRecord(TransactionCode.CRE)
            {
                SourceNumber = new ServiceNumber(serviceNumber),
                ServiceName = new ServiceName(serviceName),
                ServiceDate = new ServiceDate(serviceDate)
            };

            Assert.Equal(expectedRecord, record);
        }

        [Fact]
        public async Task MakeTransaction_SameTransactionRecordPushedAsReturned()
        {
            // Arrange
            var serviceNumber = "11211";
            var serviceName = "ANewService";
            var serviceDate = "20181001";

            var logger = new Mock<ILogger<CreateServiceTransaction>>();
            var userManager = new Mock<IUserManager>();
            var transactionQueue = new Mock<ITransactionQueue>();
            userManager.Setup(userManager => userManager.UserTransactionQueue(It.IsAny<Guid>()))
                .Returns(transactionQueue.Object);

            var command = new CreateServiceCommand(serviceNumber, serviceDate, serviceName);

            var transaction = new CreateServiceTransaction(logger.Object, userManager.Object);

            // Act
            var record = await transaction.MakeTransaction(command, Guid.NewGuid());

            // Assert
            transactionQueue.Verify(transactionQueue => transactionQueue.Push(record));
        }
    }
}
