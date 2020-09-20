using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using QIES.Common.Record;
using QIES.Core.Users;
using Xunit;

namespace QIES.Core.Services.Tests
{
    public class LogoutServiceTests
    {
        [Fact]
        public async Task DoLogout_SuccessfulLogout_ReturnedTrue()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var logger = new Mock<ILogger<LogoutService>>();
            var userManager = new Mock<IUserManager>();
            var summaryWriter = new Mock<ISummaryWriter>();
            var transactionQueue = new Mock<ITransactionQueue>();
            userManager.Setup(userManager => userManager.UserLogout(userId))
                .Returns((true, transactionQueue.Object));

            var service = new LogoutService(logger.Object, userManager.Object, summaryWriter.Object);

            // Act
            var success = await service.DoLogout(userId);

            // Assert
            Assert.True(success);
        }

        [Fact]
        public async Task DoLogout_FailedLogout_ReturnedFalse()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var logger = new Mock<ILogger<LogoutService>>();
            var userManager = new Mock<IUserManager>();
            var summaryWriter = new Mock<ISummaryWriter>();
            userManager.Setup(userManager => userManager.UserLogout(userId))
                .Returns((false, null));

            var service = new LogoutService(logger.Object, userManager.Object, summaryWriter.Object);

            // Act
            var success = await service.DoLogout(userId);

            // Assert
            Assert.False(success);
        }

        [Fact]
        public async Task DoLogout_SuccessfulLogout_PushedEOSRecordToTransactionQueue()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var logger = new Mock<ILogger<LogoutService>>();
            var userManager = new Mock<IUserManager>();
            var summaryWriter = new Mock<ISummaryWriter>();
            var transactionQueue = new Mock<ITransactionQueue>();
            userManager.Setup(userManager => userManager.UserLogout(userId))
                .Returns((true, transactionQueue.Object));

            var service = new LogoutService(logger.Object, userManager.Object, summaryWriter.Object);

            // Act
            var success = await service.DoLogout(userId);

            // Assert
            var expectedPushedRecord = new TransactionRecord(TransactionCode.EOS);
            transactionQueue.Verify(transactionQueue => transactionQueue.Push(expectedPushedRecord));
        }

        [Fact]
        public async Task DoLogout_SuccessfulLogout_SummaryFileWritten()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var logger = new Mock<ILogger<LogoutService>>();
            var userManager = new Mock<IUserManager>();
            var summaryWriter = new Mock<ISummaryWriter>();
            var transactionQueue = new Mock<ITransactionQueue>();
            userManager.Setup(userManager => userManager.UserLogout(userId))
                .Returns((true, transactionQueue.Object));

            var service = new LogoutService(logger.Object, userManager.Object, summaryWriter.Object);

            // Act
            var success = await service.DoLogout(userId);

            // Assert
            summaryWriter.Verify(summaryWriter =>
                summaryWriter.WriteTransactionSummaryFile(transactionQueue.Object, It.IsAny<string>()));
        }

        [Fact]
        public async Task DoLogout_FailedLogout_NoSummaryFileWritten()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var logger = new Mock<ILogger<LogoutService>>();
            var userManager = new Mock<IUserManager>();
            var summaryWriter = new Mock<ISummaryWriter>();
            userManager.Setup(userManager => userManager.UserLogout(userId))
                .Returns((false, null));

            var service = new LogoutService(logger.Object, userManager.Object, summaryWriter.Object);

            // Act
            var success = await service.DoLogout(userId);

            // Assert
            summaryWriter.Verify(summaryWriter =>
                summaryWriter.WriteTransactionSummaryFile(It.IsAny<ITransactionQueue>(), It.IsAny<string>()),
                Times.Never);
        }
    }
}
