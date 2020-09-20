using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using QIES.Api.Models;
using QIES.Common.Record;
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

            var logger = new Mock<ILogger<ServicesController>>();
            var servicesList = new Mock<IServicesList>();
            var userManager = new Mock<IUserManager>();
            var createServiceTransaction = new Mock<ITransaction<CreateServiceRequest>>();
            var deleteServiceTransaction = new Mock<ITransaction<DeleteServiceRequest>>();
            var sellTicketsTransaction = new Mock<ITransaction<SellTicketsCommand>>();
            var changeTicketsTransaction = new Mock<ITransaction<ChangeTicketsCommand>>();
            var cancelTicketsTransaction = new Mock<ITransaction<CancelTicketsRequest>>();

            var request = new DeleteServiceRequest();
            request.UserId = Guid.NewGuid();
            request.ServiceName = serviceName;

            userManager.Setup(userManager => userManager.IsLoggedIn(It.IsAny<Guid>()))
                .Returns(true);
            userManager.Setup(userManager => userManager.UserType(It.IsAny<Guid>()))
                .Returns(LoginType.Planner);
            servicesList.Setup(servicesList => servicesList.IsInList(serviceNumber))
                .Returns(true);
            deleteServiceTransaction.Setup(transaction => transaction.MakeTransaction(serviceNum, request, It.IsAny<Guid>()))
                .ReturnsAsync(new TransactionRecord(TransactionCode.DEL)
                    {
                        SourceNumber = serviceNumber,
                        ServiceName = new ServiceName(serviceName)
                    });

            var controller = new ServicesController(
                logger.Object,
                servicesList.Object,
                userManager.Object,
                createServiceTransaction.Object,
                deleteServiceTransaction.Object,
                sellTicketsTransaction.Object,
                changeTicketsTransaction.Object,
                cancelTicketsTransaction.Object);

            // Act
            var result = await controller.DeleteService(serviceNum, request);

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

            var logger = new Mock<ILogger<ServicesController>>();
            var servicesList = new Mock<IServicesList>();
            var userManager = new Mock<IUserManager>();
            var createServiceTransaction = new Mock<ITransaction<CreateServiceRequest>>();
            var deleteServiceTransaction = new Mock<ITransaction<DeleteServiceRequest>>();
            var sellTicketsTransaction = new Mock<ITransaction<SellTicketsCommand>>();
            var changeTicketsTransaction = new Mock<ITransaction<ChangeTicketsCommand>>();
            var cancelTicketsTransaction = new Mock<ITransaction<CancelTicketsRequest>>();

            var request = new DeleteServiceRequest();
            request.UserId = Guid.NewGuid();

            userManager.Setup(userManager => userManager.IsLoggedIn(It.IsAny<Guid>()))
                .Returns(true);
            userManager.Setup(userManager => userManager.UserType(It.IsAny<Guid>()))
                .Returns(LoginType.Agent);
            servicesList.Setup(servicesList => servicesList.IsInList(serviceNumber))
                .Returns(true);

            var controller = new ServicesController(
                logger.Object,
                servicesList.Object,
                userManager.Object,
                createServiceTransaction.Object,
                deleteServiceTransaction.Object,
                sellTicketsTransaction.Object,
                changeTicketsTransaction.Object,
                cancelTicketsTransaction.Object);

            // Act
            var result = await controller.DeleteService(serviceNum, request);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TransactionRecord>>(result);
            Assert.IsType<ForbidResult>(actionResult.Result);
        }

        [Fact]
        public async Task DeleteService_NotLoggedIn_Unauthorized()
        {
            // Arrange
            var logger = new Mock<ILogger<ServicesController>>();
            var servicesList = new Mock<IServicesList>();
            var userManager = new Mock<IUserManager>();
            var createServiceTransaction = new Mock<ITransaction<CreateServiceRequest>>();
            var deleteServiceTransaction = new Mock<ITransaction<DeleteServiceRequest>>();
            var sellTicketsTransaction = new Mock<ITransaction<SellTicketsCommand>>();
            var changeTicketsTransaction = new Mock<ITransaction<ChangeTicketsCommand>>();
            var cancelTicketsTransaction = new Mock<ITransaction<CancelTicketsRequest>>();

            var request = new CreateServiceRequest();
            request.UserId = Guid.NewGuid();

            userManager.Setup(userManager => userManager.IsLoggedIn(It.IsAny<Guid>()))
                .Returns(false);

            var controller = new ServicesController(
                logger.Object,
                servicesList.Object,
                userManager.Object,
                createServiceTransaction.Object,
                deleteServiceTransaction.Object,
                sellTicketsTransaction.Object,
                changeTicketsTransaction.Object,
                cancelTicketsTransaction.Object);

            // Act
            var result = await controller.CreateService(request);

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

            var logger = new Mock<ILogger<ServicesController>>();
            var servicesList = new Mock<IServicesList>();
            var userManager = new Mock<IUserManager>();
            var createServiceTransaction = new Mock<ITransaction<CreateServiceRequest>>();
            var deleteServiceTransaction = new Mock<ITransaction<DeleteServiceRequest>>();
            var sellTicketsTransaction = new Mock<ITransaction<SellTicketsCommand>>();
            var changeTicketsTransaction = new Mock<ITransaction<ChangeTicketsCommand>>();
            var cancelTicketsTransaction = new Mock<ITransaction<CancelTicketsRequest>>();

            var request = new DeleteServiceRequest();
            request.UserId = Guid.NewGuid();
            request.ServiceName = newServiceName;

            userManager.Setup(userManager => userManager.IsLoggedIn(It.IsAny<Guid>()))
                .Returns(true);
            userManager.Setup(userManager => userManager.UserType(It.IsAny<Guid>()))
                .Returns(LoginType.Planner);
            servicesList.Setup(servicesList => servicesList.IsInList(serviceNumber))
                .Returns(false);

            var controller = new ServicesController(
                logger.Object,
                servicesList.Object,
                userManager.Object,
                createServiceTransaction.Object,
                deleteServiceTransaction.Object,
                sellTicketsTransaction.Object,
                changeTicketsTransaction.Object,
                cancelTicketsTransaction.Object);

            // Act
            var result = await controller.DeleteService(serviceNum, request);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TransactionRecord>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }
    }
}
