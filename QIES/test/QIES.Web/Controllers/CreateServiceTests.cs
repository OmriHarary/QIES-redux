using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using QIES.Api.Models;
using QIES.Common;
using QIES.Common.Record;
using QIES.Core;
using QIES.Core.Services;
using QIES.Core.Users;
using Xunit;

namespace QIES.Web.Controllers.Tests
{
    public class CreateServiceTests
    {
        [Fact]
        public async Task CreateService_LoggedInAsPlanner_Created()
        {
            // Arrange
            var newServiceNum = "11111";
            var serviceNumber = new ServiceNumber(newServiceNum);
            var newServiceName = "New Service";
            var newServiceDate = "20201010";

            var logger = new Mock<ILogger<ServicesController>>();
            var servicesList = new Mock<IServicesList>();
            var userManager = new Mock<IUserManager>();
            var createServiceTransaction = new Mock<ITransaction<CreateServiceRequest, TransactionRecord>>();
            var deleteServiceTransaction = new Mock<ITransaction<DeleteServiceRequest, TransactionRecord>>();
            var sellOrChangeTicketsTransaction = new Mock<ITransaction<SellOrChangeTicketsRequest, TransactionRecord>>();
            var cancelTicketsTransaction = new Mock<ITransaction<CancelTicketsRequest, TransactionRecord>>();

            var request = new CreateServiceRequest();
            request.UserId = Guid.NewGuid();
            request.ServiceNumber = newServiceNum;
            request.ServiceName = newServiceName;
            request.ServiceDate = newServiceDate;

            userManager.Setup(userManager => userManager.IsLoggedIn(It.IsAny<Guid>()))
                .Returns(true);
            userManager.Setup(userManager => userManager.UserType(It.IsAny<Guid>()))
                .Returns(LoginType.Planner);
            servicesList.Setup(servicesList => servicesList.IsInList(serviceNumber))
                .Returns(false);
            createServiceTransaction.Setup(transaction => transaction.MakeTransaction(newServiceNum, request, It.IsAny<Guid>()))
                .ReturnsAsync(new TransactionRecord(TransactionCode.SEL)
                    {
                        SourceNumber = serviceNumber,
                        ServiceName = new ServiceName(newServiceName),
                        ServiceDate = new ServiceDate(newServiceDate)
                    });

            var controller = new ServicesController(
                logger.Object,
                servicesList.Object,
                userManager.Object,
                createServiceTransaction.Object,
                deleteServiceTransaction.Object,
                sellOrChangeTicketsTransaction.Object,
                cancelTicketsTransaction.Object);

            // Act
            var result = await controller.CreateService(request);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TransactionRecord>>(result);
            Assert.IsType<CreatedAtActionResult>(actionResult.Result);
        }

        [Fact]
        public async Task CreateService_LoggedInAsAgent_Forbidden()
        {
            // Arrange
            var logger = new Mock<ILogger<ServicesController>>();
            var servicesList = new Mock<IServicesList>();
            var userManager = new Mock<IUserManager>();
            var createServiceTransaction = new Mock<ITransaction<CreateServiceRequest, TransactionRecord>>();
            var deleteServiceTransaction = new Mock<ITransaction<DeleteServiceRequest, TransactionRecord>>();
            var sellOrChangeTicketsTransaction = new Mock<ITransaction<SellOrChangeTicketsRequest, TransactionRecord>>();
            var cancelTicketsTransaction = new Mock<ITransaction<CancelTicketsRequest, TransactionRecord>>();

            var request = new CreateServiceRequest();
            request.UserId = Guid.NewGuid();

            userManager.Setup(userManager => userManager.IsLoggedIn(It.IsAny<Guid>()))
                .Returns(true);
            userManager.Setup(userManager => userManager.UserType(It.IsAny<Guid>()))
                .Returns(LoginType.Agent);

            var controller = new ServicesController(
                logger.Object,
                servicesList.Object,
                userManager.Object,
                createServiceTransaction.Object,
                deleteServiceTransaction.Object,
                sellOrChangeTicketsTransaction.Object,
                cancelTicketsTransaction.Object);

            // Act
            var result = await controller.CreateService(request);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TransactionRecord>>(result);
            Assert.IsType<ForbidResult>(actionResult.Result);
        }

        [Fact]
        public async Task CreateService_NotLoggedIn_Unauthorized()
        {
            // Arrange
            var logger = new Mock<ILogger<ServicesController>>();
            var servicesList = new Mock<IServicesList>();
            var userManager = new Mock<IUserManager>();
            var createServiceTransaction = new Mock<ITransaction<CreateServiceRequest, TransactionRecord>>();
            var deleteServiceTransaction = new Mock<ITransaction<DeleteServiceRequest, TransactionRecord>>();
            var sellOrChangeTicketsTransaction = new Mock<ITransaction<SellOrChangeTicketsRequest, TransactionRecord>>();
            var cancelTicketsTransaction = new Mock<ITransaction<CancelTicketsRequest, TransactionRecord>>();

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
                sellOrChangeTicketsTransaction.Object,
                cancelTicketsTransaction.Object);

            // Act
            var result = await controller.CreateService(request);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TransactionRecord>>(result);
            Assert.IsType<UnauthorizedResult>(actionResult.Result);
        }

        [Fact]
        public async Task CreateService_ServiceAlreadyExists_Conflict()
        {
            // Arrange
            var newServiceNum = "11111";
            var serviceNumber = new ServiceNumber(newServiceNum);
            var newServiceName = "New Service";
            var newServiceDate = "20201010";

            var logger = new Mock<ILogger<ServicesController>>();
            var servicesList = new Mock<IServicesList>();
            var userManager = new Mock<IUserManager>();
            var createServiceTransaction = new Mock<ITransaction<CreateServiceRequest, TransactionRecord>>();
            var deleteServiceTransaction = new Mock<ITransaction<DeleteServiceRequest, TransactionRecord>>();
            var sellOrChangeTicketsTransaction = new Mock<ITransaction<SellOrChangeTicketsRequest, TransactionRecord>>();
            var cancelTicketsTransaction = new Mock<ITransaction<CancelTicketsRequest, TransactionRecord>>();

            var request = new CreateServiceRequest();
            request.UserId = Guid.NewGuid();
            request.ServiceNumber = newServiceNum;
            request.ServiceName = newServiceName;
            request.ServiceDate = newServiceDate;

            userManager.Setup(userManager => userManager.IsLoggedIn(It.IsAny<Guid>()))
                .Returns(true);
            userManager.Setup(userManager => userManager.UserType(It.IsAny<Guid>()))
                .Returns(LoginType.Planner);
            servicesList.Setup(servicesList => servicesList.IsInList(serviceNumber))
                .Returns(true);

            var controller = new ServicesController(
                logger.Object,
                servicesList.Object,
                userManager.Object,
                createServiceTransaction.Object,
                deleteServiceTransaction.Object,
                sellOrChangeTicketsTransaction.Object,
                cancelTicketsTransaction.Object);

            // Act
            var result = await controller.CreateService(request);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TransactionRecord>>(result);
            Assert.IsType<ConflictResult>(actionResult.Result);
        }
    }
}
