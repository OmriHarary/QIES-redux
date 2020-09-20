using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using QIES.Api.Models;
using QIES.Core.Services;
using Xunit;

namespace QIES.Web.Controllers.Tests
{
    public class LogoutTests
    {
        [Fact]
        public async Task Logout_Success_Ok()
        {
            // Arrange
            var logger = new Mock<ILogger<UsersController>>();
            var logoutService = new Mock<ILogoutService>();
            var loginService = new Mock<ILoginService>();

            logoutService.Setup(logoutService => logoutService.DoLogout(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            var controller = new UsersController(logger.Object, loginService.Object, logoutService.Object);

            var logoutRequest = new LogoutRequest();
            logoutRequest.UserId = Guid.NewGuid();

            // Act
            var result = await controller.Logout(logoutRequest);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Logout_Failure_BadRequest()
        {
            // Arrange
            var logger = new Mock<ILogger<UsersController>>();
            var logoutService = new Mock<ILogoutService>();
            var loginService = new Mock<ILoginService>();

            logoutService.Setup(logoutService => logoutService.DoLogout(It.IsAny<Guid>()))
                .ReturnsAsync(false);

            var controller = new UsersController(logger.Object, loginService.Object, logoutService.Object);

            var logoutRequest = new LogoutRequest();
            logoutRequest.UserId = Guid.NewGuid();

            // Act
            var result = await controller.Logout(logoutRequest);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
    }
}
