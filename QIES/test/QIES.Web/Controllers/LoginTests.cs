using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using QIES.Api.Models;
using QIES.Api.Responses;
using QIES.Core.Services;
using QIES.Core.Users;
using Xunit;

namespace QIES.Web.Controllers.Tests
{
    public class LoginTests
    {
        [Fact]
        public async Task Login_AsAgent_Successful()
        {
            // Arrange
            var stubLogger = new Mock<ILogger<UsersController>>();
            var stubLogoutService = new Mock<ILogoutService>();
            var mockLoginService = new Mock<ILoginService>();
            mockLoginService.Setup(loginService => loginService.DoLogin(LoginType.Agent))
                .ReturnsAsync(new Agent());

            var controller = new UsersController(stubLogger.Object, mockLoginService.Object, stubLogoutService.Object);

            var loginRequest = new LoginRequest();
            loginRequest.Login = "agent";

            // Act
            var result = await controller.Login(loginRequest);

            // Assert
            var actionResult = Assert.IsType<ActionResult<LoginResponse>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var response = Assert.IsType<LoginResponse>(okObjectResult.Value);
            Assert.Equal((int)LoginType.Agent, response.Type);
        }

        [Fact]
        public async Task Login_AsPlanner_Successful()
        {
            // Arrange
            var stubLogger = new Mock<ILogger<UsersController>>();
            var stubLogoutService = new Mock<ILogoutService>();
            var mockLoginService = new Mock<ILoginService>();
            mockLoginService.Setup(loginService => loginService.DoLogin(LoginType.Planner))
                .ReturnsAsync(new Planner());

            var controller = new UsersController(stubLogger.Object, mockLoginService.Object, stubLogoutService.Object);

            var loginRequest = new LoginRequest();
            loginRequest.Login = "planner";

            // Act
            var result = await controller.Login(loginRequest);

            // Assert
            var actionResult = Assert.IsType<ActionResult<LoginResponse>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var response = Assert.IsType<LoginResponse>(okObjectResult.Value);
            Assert.Equal((int)LoginType.Planner, response.Type);
        }

        [Fact]
        public async Task Login_AsAnythingElse_BadRequest()
        {
            // Arrange
            var stubLogger = new Mock<ILogger<UsersController>>();
            var stubLogoutService = new Mock<ILogoutService>();
            var stubLoginService = new Mock<ILoginService>();

            var controller = new UsersController(stubLogger.Object, stubLoginService.Object, stubLogoutService.Object);

            var loginRequest = new LoginRequest();
            loginRequest.Login = "other";

            // Act
            var result = await controller.Login(loginRequest);

            // Assert
            var actionResult = Assert.IsType<ActionResult<LoginResponse>>(result);
            Assert.IsType<BadRequestResult>(actionResult.Result);
        }
    }
}
