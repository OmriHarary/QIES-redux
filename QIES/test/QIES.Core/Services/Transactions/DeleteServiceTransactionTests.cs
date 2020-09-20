using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using QIES.Api.Models;
using QIES.Common.Record;
using QIES.Core.Users;
using Xunit;

namespace QIES.Core.Services.Tests
{
    public class DeleteServiceTransactionTests
    {
        [Fact]
        public async Task MakeTransaction_CorrectTransactionRecordCreated()
        {
            // Arrange
            var serviceNumber = "11211";
            var serviceName = "ANewService";

            var logger = new Mock<ILogger<DeleteServiceTransaction>>();
            var userManager = new Mock<IUserManager>();
            var transactionQueue = new Mock<ITransactionQueue>();
            userManager.Setup(userManager => userManager.UserTransactionQueue(It.IsAny<Guid>()))
                .Returns(transactionQueue.Object);

            var request = new DeleteServiceRequest()
            {
                ServiceName = serviceName
            };

            var transaction = new DeleteServiceTransaction(logger.Object, userManager.Object);

            // Act
            var record = await transaction.MakeTransaction(serviceNumber, request, Guid.NewGuid());

            // Assert
            var expectedRecord = new TransactionRecord(TransactionCode.DEL)
            {
                SourceNumber = new ServiceNumber(serviceNumber),
                ServiceName = new ServiceName(serviceName)
            };

            Assert.Equal(expectedRecord, record);
        }

        [Fact]
        public async Task MakeTransaction_SameTransactionRecordPushedAsReturned()
        {
            // Arrange
            var serviceNumber = "11211";
            var serviceName = "ANewService";

            var logger = new Mock<ILogger<DeleteServiceTransaction>>();
            var userManager = new Mock<IUserManager>();
            var transactionQueue = new Mock<ITransactionQueue>();
            userManager.Setup(userManager => userManager.UserTransactionQueue(It.IsAny<Guid>()))
                .Returns(transactionQueue.Object);

            var command = new DeleteServiceRequest()
            {
                ServiceName = serviceName
            };

            var transaction = new DeleteServiceTransaction(logger.Object, userManager.Object);

            // Act
            var record = await transaction.MakeTransaction(serviceNumber, command, Guid.NewGuid());

            // Assert
            transactionQueue.Verify(transactionQueue => transactionQueue.Push(record));
        }
    }
}
