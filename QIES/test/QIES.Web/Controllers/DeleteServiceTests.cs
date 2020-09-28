using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using QIES.Api.Models;
using QIES.Common.Records;
using QIES.Core;
using QIES.Core.Commands;
using QIES.Core.Services;
using QIES.Core.Users;
using Xunit;

namespace QIES.Web.Controllers.Tests
{
    public class DeleteServiceTests
    {
        [Fact]
        public async Task DeleteService_LoggedInAsPlanner_Deleted()
        {
            // Arrange
            var serviceNum = "11111";
            var serviceNumber = new ServiceNumber(serviceNum);
            var serviceName = "A Service";
            var userId = Guid.NewGuid();

            var logger = new Mock<ILogger<ServicesController>>();
            var servicesList = new Mock<IServicesList>();
            var userManager = new Mock<IUserManager>();
            var deleteServiceTransaction = new Mock<ITransaction<DeleteServiceCommand>>();

            var request = new DeleteServiceRequest
            {
                ServiceName = serviceName
            };

            userManager.Setup(userManager => userManager.IsLoggedIn(It.IsAny<Guid>()))
                .Returns(true);
            userManager.Setup(userManager => userManager.UserType(It.IsAny<Guid>()))
                .Returns(LoginType.Planner);
            servicesList.Setup(servicesList => servicesList.IsInList(serviceNumber))
                .Returns(true);
            deleteServiceTransaction.Setup(transaction => transaction.MakeTransaction(It.IsAny<DeleteServiceCommand>(), It.IsAny<Guid>()))
                .ReturnsAsync(new TransactionRecord(TransactionCode.DEL)
                    {
                        SourceNumber = serviceNumber,
                        ServiceName = new ServiceName(serviceName)
                    });

            var controller = new ServicesController(
                logger.Object,
                servicesList.Object,
                userManager.Object);

            // Act
            var result = await controller.DeleteService(serviceNum, request, userId, deleteServiceTransaction.Object);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TransactionRecord>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.IsType<TransactionRecord>(okObjectResult.Value);
        }

        [Fact]
        public async Task DeleteService_LoggedInAsAgent_Forbidden()
        {
            // Arrange
            var serviceNum = "11111";
            var serviceNumber = new ServiceNumber(serviceNum);
            var serviceName = "A Service";
            var userId = Guid.NewGuid();

            var logger = new Mock<ILogger<ServicesController>>();
            var servicesList = new Mock<IServicesList>();
            var userManager = new Mock<IUserManager>();
            var deleteServiceTransaction = new Mock<ITransaction<DeleteServiceCommand>>();

            var request = new DeleteServiceRequest
            {
                ServiceName = serviceName
            };

            userManager.Setup(userManager => userManager.IsLoggedIn(It.IsAny<Guid>()))
                .Returns(true);
            userManager.Setup(userManager => userManager.UserType(It.IsAny<Guid>()))
                .Returns(LoginType.Agent);
            servicesList.Setup(servicesList => servicesList.IsInList(serviceNumber))
                .Returns(true);

            var controller = new ServicesController(
                logger.Object,
                servicesList.Object,
                userManager.Object);

            // Act
            var result = await controller.DeleteService(serviceNum, request, userId, deleteServiceTransaction.Object);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TransactionRecord>>(result);
            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status403Forbidden, objectResult.StatusCode);
        }

        [Fact]
        public async Task DeleteService_NotLoggedIn_Unauthorized()
        {
            // Arrange
            var serviceNum = "11111";
            var serviceName = "A Service";
            var userId = Guid.NewGuid();

            var logger = new Mock<ILogger<ServicesController>>();
            var servicesList = new Mock<IServicesList>();
            var userManager = new Mock<IUserManager>();
            var deleteServiceTransaction = new Mock<ITransaction<DeleteServiceCommand>>();

            var request = new DeleteServiceRequest
            {
                ServiceName = serviceName
            };

            userManager.Setup(userManager => userManager.IsLoggedIn(It.IsAny<Guid>()))
                .Returns(false);

            var controller = new ServicesController(
                logger.Object,
                servicesList.Object,
                userManager.Object);

            // Act
            var result = await controller.DeleteService(serviceNum, request, userId, deleteServiceTransaction.Object);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TransactionRecord>>(result);
            Assert.IsType<UnauthorizedResult>(actionResult.Result);
        }

        [Fact]
        public async Task DeleteService_NoUserId_Unauthorized()
        {
            // Arrange
            var serviceNum = "11111";
            var serviceName = "A Service";
            Guid? userId = null;

            var logger = new Mock<ILogger<ServicesController>>();
            var servicesList = new Mock<IServicesList>();
            var userManager = new Mock<IUserManager>();
            var deleteServiceTransaction = new Mock<ITransaction<DeleteServiceCommand>>();

            var request = new DeleteServiceRequest
            {
                ServiceName = serviceName
            };

            var controller = new ServicesController(
                logger.Object,
                servicesList.Object,
                userManager.Object);

            // Act
            var result = await controller.DeleteService(serviceNum, request, userId, deleteServiceTransaction.Object);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TransactionRecord>>(result);
            Assert.IsType<UnauthorizedResult>(actionResult.Result);
        }

        [Fact]
        public async Task DeleteService_ServiceDoesNotExist_NotFound()
        {
            // Arrange
            var serviceNum = "11111";
            var serviceNumber = new ServiceNumber(serviceNum);
            var newServiceName = "New Service";
            var userId = Guid.NewGuid();

            var logger = new Mock<ILogger<ServicesController>>();
            var servicesList = new Mock<IServicesList>();
            var userManager = new Mock<IUserManager>();
            var deleteServiceTransaction = new Mock<ITransaction<DeleteServiceCommand>>();

            var request = new DeleteServiceRequest
            {
                ServiceName = newServiceName
            };

            userManager.Setup(userManager => userManager.IsLoggedIn(It.IsAny<Guid>()))
                .Returns(true);
            userManager.Setup(userManager => userManager.UserType(It.IsAny<Guid>()))
                .Returns(LoginType.Planner);
            servicesList.Setup(servicesList => servicesList.IsInList(serviceNumber))
                .Returns(false);

            var controller = new ServicesController(
                logger.Object,
                servicesList.Object,
                userManager.Object);

            // Act
            var result = await controller.DeleteService(serviceNum, request, userId, deleteServiceTransaction.Object);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TransactionRecord>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }
    }
}
