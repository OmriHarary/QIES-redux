using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using QIES.Core.Users;
using Xunit;

namespace QIES.Core.Services.Tests
{
    public class LoginServiceTests
    {
        [Fact]
        public async Task DoLogin_LoginAsAgent()
        {
            // Arrange
            var logger = new Mock<ILogger<LoginService>>();
            var userManager = new Mock<IUserManager>();
            userManager.Setup(userManager => userManager.UserLogin(LoginType.Agent))
                .Returns(new Agent());

            var service = new LoginService(logger.Object, userManager.Object);

            // Act
            var user = await service.DoLogin(LoginType.Agent);

            // Assert
            Assert.IsType<Agent>(user);
        }

        [Fact]
        public async Task DoLogin_LoginAsPlanner()
        {
            // Arrange
            var logger = new Mock<ILogger<LoginService>>();
            var userManager = new Mock<IUserManager>();
            userManager.Setup(userManager => userManager.UserLogin(LoginType.Planner))
                .Returns(new Planner());

            var service = new LoginService(logger.Object, userManager.Object);

            // Act
            var user = await service.DoLogin(LoginType.Planner);

            // Assert
            Assert.IsType<Planner>(user);
        }
    }
}
