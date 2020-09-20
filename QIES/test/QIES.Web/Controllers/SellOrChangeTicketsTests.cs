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
    public class SellOrChangeTicketTests
    {
        [Fact]
        public async Task SellOrChangeTicket_NotLoggedIn_Unauthorized()
        {
            // Arrange
            var serviceNumber = "11111";
            var numberTickets = "1";

            var logger = new Mock<ILogger<ServicesController>>();
            var servicesList = new Mock<IServicesList>();
            var userManager = new Mock<IUserManager>();
            var sellTicketsTransaction = new Mock<ITransaction<SellTicketsCommand>>();
            var changeTicketsTransaction = new Mock<ITransaction<ChangeTicketsCommand>>();

            var request = new SellOrChangeTicketsRequest
            {
                NumberTickets = numberTickets,
                UserId = Guid.NewGuid()
            };

            userManager.Setup(userManager => userManager.IsLoggedIn(It.IsAny<Guid>()))
                .Returns(false);

            var controller = new ServicesController(
                logger.Object,
                servicesList.Object,
                userManager.Object);

            // Act
            var result = await controller.SellOrChangeTickets(serviceNumber, request, sellTicketsTransaction.Object, changeTicketsTransaction.Object);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TransactionRecord>>(result);
            Assert.IsType<UnauthorizedResult>(actionResult.Result);
        }

        [Fact]
        public async Task SellOrChangeTicket_RequestWithNoSourceServiceNumber_DoesSuccessfulSellTicket()
        {
            // Arrange
            var serviceNum = "11111";
            var serviceNumber = new ServiceNumber(serviceNum);
            var numberTickets = "1";

            var logger = new Mock<ILogger<ServicesController>>();
            var servicesList = new Mock<IServicesList>();
            var userManager = new Mock<IUserManager>();
            var sellTicketsTransaction = new Mock<ITransaction<SellTicketsCommand>>();
            var changeTicketsTransaction = new Mock<ITransaction<ChangeTicketsCommand>>();

            var request = new SellOrChangeTicketsRequest
            {
                NumberTickets = numberTickets,
                UserId = Guid.NewGuid()
            };

            userManager.Setup(userManager => userManager.IsLoggedIn(It.IsAny<Guid>()))
                .Returns(true);
            servicesList.Setup(servicesList => servicesList.IsInList(serviceNumber))
                .Returns(true);
            sellTicketsTransaction.Setup(transaction => transaction.MakeTransaction(serviceNum, It.IsAny<SellTicketsCommand>(), It.IsAny<Guid>()))
                .ReturnsAsync(new TransactionRecord(TransactionCode.SEL)
                    {
                        SourceNumber = serviceNumber,
                        NumberTickets = new NumberTickets(int.Parse(numberTickets))
                    })
                .Verifiable();

            var controller = new ServicesController(
                logger.Object,
                servicesList.Object,
                userManager.Object);

            // Act
            var result = await controller.SellOrChangeTickets(serviceNum, request, sellTicketsTransaction.Object, changeTicketsTransaction.Object);

            // Assert
            sellTicketsTransaction.Verify();
            changeTicketsTransaction.Verify(transaction => transaction.MakeTransaction(
                    It.IsAny<string>(),
                    It.IsAny<ChangeTicketsCommand>(),
                    It.IsAny<Guid>()),
                Times.Never());
            var actionResult = Assert.IsType<ActionResult<TransactionRecord>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.IsType<TransactionRecord>(okObjectResult.Value);
        }

        [Fact]
        public async Task SellOrChangeTicket_RequestWithSourceServiceNumber_DoesSuccessfulChangeTicket()
        {
            // Arrange
            var sourceNum = "11111";
            var sourceServiceNumber = new ServiceNumber(sourceNum);
            var destinationNum = "11112";
            var destinationServiceNumber = new ServiceNumber(destinationNum);
            var numberTickets = "1";

            var logger = new Mock<ILogger<ServicesController>>();
            var servicesList = new Mock<IServicesList>();
            var userManager = new Mock<IUserManager>();
            var sellTicketsTransaction = new Mock<ITransaction<SellTicketsCommand>>();
            var changeTicketsTransaction = new Mock<ITransaction<ChangeTicketsCommand>>();

            var request = new SellOrChangeTicketsRequest
            {
                SourceServiceNumber = sourceNum,
                NumberTickets = numberTickets,
                UserId = Guid.NewGuid()
            };

            userManager.Setup(userManager => userManager.IsLoggedIn(It.IsAny<Guid>()))
                .Returns(true);
            servicesList.Setup(servicesList => servicesList.IsInList(sourceServiceNumber))
                .Returns(true);
            servicesList.Setup(servicesList => servicesList.IsInList(destinationServiceNumber))
                .Returns(true);
            changeTicketsTransaction.Setup(transaction => transaction.MakeTransaction(destinationNum, It.IsAny<ChangeTicketsCommand>(), It.IsAny<Guid>()))
                .ReturnsAsync(new TransactionRecord(TransactionCode.CHG)
                    {
                        SourceNumber = sourceServiceNumber,
                        NumberTickets = new NumberTickets(int.Parse(numberTickets)),
                        DestinationNumber = destinationServiceNumber
                    })
                .Verifiable();

            var controller = new ServicesController(
                logger.Object,
                servicesList.Object,
                userManager.Object);

            // Act
            var result = await controller.SellOrChangeTickets(destinationNum, request, sellTicketsTransaction.Object, changeTicketsTransaction.Object);

            // Assert
            changeTicketsTransaction.Verify();
            sellTicketsTransaction.Verify(transaction => transaction.MakeTransaction(
                    It.IsAny<string>(),
                    It.IsAny<SellTicketsCommand>(),
                    It.IsAny<Guid>()),
                Times.Never());
            var actionResult = Assert.IsType<ActionResult<TransactionRecord>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.IsType<TransactionRecord>(okObjectResult.Value);
        }

        [Fact]
        public async Task SellOrChangeTicket_SellWhenServiceNumberDoesNotExist_NotFound()
        {
            // Arrange
            var serviceNum = "11111";
            var serviceNumber = new ServiceNumber(serviceNum);
            var numberTickets = "1";

            var logger = new Mock<ILogger<ServicesController>>();
            var servicesList = new Mock<IServicesList>();
            var userManager = new Mock<IUserManager>();
            var sellTicketsTransaction = new Mock<ITransaction<SellTicketsCommand>>();
            var changeTicketsTransaction = new Mock<ITransaction<ChangeTicketsCommand>>();

            var request = new SellOrChangeTicketsRequest
            {
                NumberTickets = numberTickets,
                UserId = Guid.NewGuid()
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
            var result = await controller.SellOrChangeTickets(serviceNum, request, sellTicketsTransaction.Object, changeTicketsTransaction.Object);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TransactionRecord>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task SellOrChangeTicket_ChangeWhenSourceNumberDoesNotExist_NotFound()
        {
            // Arrange
            var sourceNum = "11111";
            var sourceServiceNumber = new ServiceNumber(sourceNum);
            var destinationNum = "11112";
            var destinationServiceNumber = new ServiceNumber(destinationNum);
            var numberTickets = "1";

            var logger = new Mock<ILogger<ServicesController>>();
            var servicesList = new Mock<IServicesList>();
            var userManager = new Mock<IUserManager>();
            var sellTicketsTransaction = new Mock<ITransaction<SellTicketsCommand>>();
            var changeTicketsTransaction = new Mock<ITransaction<ChangeTicketsCommand>>();

            var request = new SellOrChangeTicketsRequest
            {
                SourceServiceNumber = sourceNum,
                NumberTickets = numberTickets,
                UserId = Guid.NewGuid()
            };

            userManager.Setup(userManager => userManager.IsLoggedIn(It.IsAny<Guid>()))
                .Returns(true);
            servicesList.Setup(servicesList => servicesList.IsInList(sourceServiceNumber))
                .Returns(false);
            servicesList.Setup(servicesList => servicesList.IsInList(destinationServiceNumber))
                .Returns(true);

            var controller = new ServicesController(
                logger.Object,
                servicesList.Object,
                userManager.Object);

            // Act
            var result = await controller.SellOrChangeTickets(destinationNum, request, sellTicketsTransaction.Object, changeTicketsTransaction.Object);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TransactionRecord>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task SellOrChangeTicket_ChangeWhenDestinationNumberDoesNotExist_NotFound()
        {
            // Arrange
            var sourceNum = "11111";
            var sourceServiceNumber = new ServiceNumber(sourceNum);
            var destinationNum = "11112";
            var destinationServiceNumber = new ServiceNumber(destinationNum);
            var numberTickets = "1";

            var logger = new Mock<ILogger<ServicesController>>();
            var servicesList = new Mock<IServicesList>();
            var userManager = new Mock<IUserManager>();
            var sellTicketsTransaction = new Mock<ITransaction<SellTicketsCommand>>();
            var changeTicketsTransaction = new Mock<ITransaction<ChangeTicketsCommand>>();

            var request = new SellOrChangeTicketsRequest
            {
                SourceServiceNumber = sourceNum,
                NumberTickets = numberTickets,
                UserId = Guid.NewGuid()
            };

            userManager.Setup(userManager => userManager.IsLoggedIn(It.IsAny<Guid>()))
                .Returns(true);
            servicesList.Setup(servicesList => servicesList.IsInList(sourceServiceNumber))
                .Returns(true);
            servicesList.Setup(servicesList => servicesList.IsInList(destinationServiceNumber))
                .Returns(false);

            var controller = new ServicesController(
                logger.Object,
                servicesList.Object,
                userManager.Object);

            // Act
            var result = await controller.SellOrChangeTickets(destinationNum, request, sellTicketsTransaction.Object, changeTicketsTransaction.Object);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TransactionRecord>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task SellOrChangeTicket_ChangeTicketAgentLimitExceeded_TooManyRequests()
        {
            // Arrange
            var sourceNum = "11111";
            var sourceServiceNumber = new ServiceNumber(sourceNum);
            var destinationNum = "11112";
            var destinationServiceNumber = new ServiceNumber(destinationNum);
            var numberTickets = "21";

            var logger = new Mock<ILogger<ServicesController>>();
            var servicesList = new Mock<IServicesList>();
            var userManager = new Mock<IUserManager>();
            var sellTicketsTransaction = new Mock<ITransaction<SellTicketsCommand>>();
            var changeTicketsTransaction = new Mock<ITransaction<ChangeTicketsCommand>>();

            var request = new SellOrChangeTicketsRequest
            {
                SourceServiceNumber = sourceNum,
                NumberTickets = numberTickets,
                UserId = Guid.NewGuid()
            };

            userManager.Setup(userManager => userManager.IsLoggedIn(It.IsAny<Guid>()))
                .Returns(true);
            servicesList.Setup(servicesList => servicesList.IsInList(sourceServiceNumber))
                .Returns(true);
            servicesList.Setup(servicesList => servicesList.IsInList(destinationServiceNumber))
                .Returns(true);
            changeTicketsTransaction.Setup(transaction => transaction.MakeTransaction(destinationNum, It.IsAny<ChangeTicketsCommand>(), It.IsAny<Guid>()))
                .ThrowsAsync(new AgentLimitExceededException("Cannot change as total session changed tickets would be over 20. User has 20 tickets left to cancel this session."));

            var controller = new ServicesController(
                logger.Object,
                servicesList.Object,
                userManager.Object);

            // Act
            var result = await controller.SellOrChangeTickets(destinationNum, request, sellTicketsTransaction.Object, changeTicketsTransaction.Object);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TransactionRecord>>(result);
            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status429TooManyRequests, objectResult.StatusCode);
        }
    }
}
