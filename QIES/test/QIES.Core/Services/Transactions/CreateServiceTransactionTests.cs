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
    public class CreateServiceTransactionTests
    {
        [Fact]
        public async Task MakeTransaction_CorrectTransactionRecordCreated()
        {
            // Arrange
            var serviceNum = "11211";
            var serviceNumber = new ServiceNumber(serviceNum);
            var serviceName = new ServiceName("ANewService");
            var serviceDate = new ServiceDate("20181001");

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
                SourceNumber = serviceNumber,
                ServiceName = serviceName,
                ServiceDate = serviceDate
            };

            Assert.Equal(expectedRecord, record);
        }

        [Fact]
        public async Task MakeTransaction_SameTransactionRecordPushedAsReturned()
        {
            // Arrange
            var serviceNum = "11211";
            var serviceNumber = new ServiceNumber(serviceNum);
            var serviceName = new ServiceName("ANewService");
            var serviceDate = new ServiceDate("20181001");

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
