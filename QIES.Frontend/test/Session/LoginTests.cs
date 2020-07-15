using Xunit;

namespace QIES.Frontend.Session.Tests
{
    public class LoginTests
    {
        private SessionController controller;

        public LoginTests()
        {
            controller = new SessionController();
        }

        [Fact]
        public void Login_AsAgent_Success()
        {
            var request = "agent";

            var (success, _, login) = controller.ProcessLogin(request);

            Assert.True(success);
            Assert.Equal(LoginType.AGENT, login);
        }

        [Fact]
        public void Login_AsPlanner_Success()
        {
            var request = "planner";

            var (success, _, login) = controller.ProcessLogin(request);

            Assert.True(success);
            Assert.Equal(LoginType.PLANNER, login);
        }

        [Fact]
        public void Login_AsOther_Failure()
        {
            var request = "other";

            var (success, _, login) = controller.ProcessLogin(request);

            Assert.False(success);
            Assert.Equal(LoginType.NONE, login);
        }

        [Fact]
        public void Login_AlreadyLoggedIn_Failure()
        {
            controller.ActiveLogin = LoginType.AGENT;
            var request = "planner";

            var (success, _, login) = controller.ProcessLogin(request);

            Assert.False(success);
            Assert.Equal(LoginType.AGENT, login);
        }
    }
}
