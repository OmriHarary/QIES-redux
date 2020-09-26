using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
    public class CancelTicketTests
    {
        [Fact]
        public async Task CancelTickets_SuccessfullyCancelled()
        {
            // Arrange
            var serviceNum = "11111";
            var serviceNumber = new ServiceNumber(serviceNum);
            var numberTickets = "1";
            var userId = Guid.NewGuid();

            var logger = new Mock<ILogger<ServicesController>>();
            var servicesList = new Mock<IServicesList>();
            var userManager = new Mock<IUserManager>();
            var cancelTicketsTransaction = new Mock<ITransaction<CancelTicketsCommand>>();

            var request = new CancelTicketsRequest
            {
                NumberTickets = numberTickets
            };

            userManager.Setup(userManager => userManager.IsLoggedIn(It.IsAny<Guid>()))
                .Returns(true);
            servicesList.Setup(servicesList => servicesList.IsInList(serviceNumber))
                .Returns(true);
            cancelTicketsTransaction.Setup(transaction => transaction.MakeTransaction(It.IsAny<CancelTicketsCommand>(), It.IsAny<Guid>()))
                .ReturnsAsync(new TransactionRecord(TransactionCode.CAN)
                    {
                        SourceNumber = serviceNumber,
                        NumberTickets = new NumberTickets(int.Parse(numberTickets))
                    });

            var controller = new ServicesController(
                logger.Object,
                servicesList.Object,
                userManager.Object);

            // Act
            var result = await controller.CancelTickets(serviceNum, request, userId, cancelTicketsTransaction.Object);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TransactionRecord>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.IsType<TransactionRecord>(okObjectResult.Value);
        }

        [Fact]
        public async Task CancelTickets_NotLoggedIn_Unauthorized()
        {
            // Arrange
            var serviceNumber = "11111";
            var numberTickets = "1";
            var userId = Guid.NewGuid();

            var logger = new Mock<ILogger<ServicesController>>();
            var servicesList = new Mock<IServicesList>();
            var userManager = new Mock<IUserManager>();
            var cancelTicketsTransaction = new Mock<ITransaction<CancelTicketsCommand>>();

            var request = new CancelTicketsRequest
            {
                NumberTickets = numberTickets
            };

            userManager.Setup(userManager => userManager.IsLoggedIn(It.IsAny<Guid>()))
                .Returns(false);

            var controller = new ServicesController(
                logger.Object,
                servicesList.Object,
                userManager.Object);

            // Act
            var result = await controller.CancelTickets(serviceNumber, request, userId, cancelTicketsTransaction.Object);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TransactionRecord>>(result);
            Assert.IsType<UnauthorizedResult>(actionResult.Result);
        }

        [Fact]
        public async Task CancelTickets_NoUserId_Unauthorized()
        {
            // Arrange
            var serviceNumber = "11111";
            var numberTickets = "1";
            Guid? userId = null;

            var logger = new Mock<ILogger<ServicesController>>();
            var servicesList = new Mock<IServicesList>();
            var userManager = new Mock<IUserManager>();
            var cancelTicketsTransaction = new Mock<ITransaction<CancelTicketsCommand>>();

            var request = new CancelTicketsRequest
            {
                NumberTickets = numberTickets
            };

            var controller = new ServicesController(
                logger.Object,
                servicesList.Object,
                userManager.Object);

            // Act
            var result = await controller.CancelTickets(serviceNumber, request, userId, cancelTicketsTransaction.Object);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TransactionRecord>>(result);
            Assert.IsType<UnauthorizedResult>(actionResult.Result);
        }

        [Fact]
        public async Task CancelTickets_ServiceDoesNotExist_NotFound()
        {
            // Arrange
            var serviceNum = "11111";
            var serviceNumber = new ServiceNumber(serviceNum);
            var numberTickets = "1";
            var userId = Guid.NewGuid();

            var logger = new Mock<ILogger<ServicesController>>();
            var servicesList = new Mock<IServicesList>();
            var userManager = new Mock<IUserManager>();
            var cancelTicketsTransaction = new Mock<ITransaction<CancelTicketsCommand>>();

            var request = new CancelTicketsRequest
            {
                NumberTickets = numberTickets
            };

            userManager.Setup(userManager => userManager.IsLoggedIn(It.IsAny<Guid>()))
                .Returns(true);
            servicesList.Setup(servicesList => servicesList.IsInList(serviceNumber))
                .Returns(false);

            var controller = new ServicesController(
                logger.Object,
                servicesList.Object,
                userManager.Object);

            // Act
            var result = await controller.CancelTickets(serviceNum, request, userId, cancelTicketsTransaction.Object);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TransactionRecord>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task CancelTickets_CancelTicketAgentServiceLimitExceeded_TooManyRequests()
        {
            // Arrange
            var serviceNum = "11111";
            var serviceNumber = new ServiceNumber(serviceNum);
            var numberTickets = "11";
            var userId = Guid.NewGuid();

            var logger = new Mock<ILogger<ServicesController>>();
            var servicesList = new Mock<IServicesList>();
            var userManager = new Mock<IUserManager>();
            var cancelTicketsTransaction = new Mock<ITransaction<CancelTicketsCommand>>();

            var request = new CancelTicketsRequest
            {
                NumberTickets = numberTickets
            };

            userManager.Setup(userManager => userManager.IsLoggedIn(It.IsAny<Guid>()))
                .Returns(true);
            servicesList.Setup(servicesList => servicesList.IsInList(serviceNumber))
                .Returns(true);
            cancelTicketsTransaction.Setup(transaction => transaction.MakeTransaction(It.IsAny<CancelTicketsCommand>(), It.IsAny<Guid>()))
                .ThrowsAsync(new AgentLimitExceededException("Cannot cancel more then 10 tickets for a single service. User has 10 tickets left to cancel for this service."));

            var controller = new ServicesController(
                logger.Object,
                servicesList.Object,
                userManager.Object);

            // Act
            var result = await controller.CancelTickets(serviceNum, request, userId, cancelTicketsTransaction.Object);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TransactionRecord>>(result);
            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status429TooManyRequests, objectResult.StatusCode);
        }

        [Fact]
        public async Task CancelTickets_CancelTicketAgentSessionLimitExceeded_TooManyRequests()
        {
            // Arrange
            var serviceNum = "11111";
            var serviceNumber = new ServiceNumber(serviceNum);
            var numberTickets = "21";
            var userId = Guid.NewGuid();

            var logger = new Mock<ILogger<ServicesController>>();
            var servicesList = new Mock<IServicesList>();
            var userManager = new Mock<IUserManager>();
            var cancelTicketsTransaction = new Mock<ITransaction<CancelTicketsCommand>>();

            var request = new CancelTicketsRequest
            {
                NumberTickets = numberTickets
            };

            userManager.Setup(userManager => userManager.IsLoggedIn(It.IsAny<Guid>()))
                .Returns(true);
            servicesList.Setup(servicesList => servicesList.IsInList(serviceNumber))
                .Returns(true);
            cancelTicketsTransaction.Setup(transaction => transaction.MakeTransaction(It.IsAny<CancelTicketsCommand>(), It.IsAny<Guid>()))
                .ThrowsAsync(new AgentLimitExceededException("Cannot cancel as total session canceled tickets would be over 20. User has 20 tickets left to cancel this session."));

            var controller = new ServicesController(
                logger.Object,
                servicesList.Object,
                userManager.Object);

            // Act
            var result = await controller.CancelTickets(serviceNum, request, userId, cancelTicketsTransaction.Object);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TransactionRecord>>(result);
            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status429TooManyRequests, objectResult.StatusCode);
        }
    }
}
