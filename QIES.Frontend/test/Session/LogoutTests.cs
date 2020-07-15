using QIES.Frontend.Transaction;
using Xunit;

namespace QIES.Frontend.Session.Tests
{
    public class LogoutTests
    {
        private SessionController controller;

        public LogoutTests()
        {
            controller = new SessionController();
        }

        [Fact]
        public void Logout_AsAgent_LogoutSuccess()
        {
            controller.ActiveLogin = LoginType.AGENT;
            var request = new LogoutRequest(); 

            var (success, _, login) = controller.ProcessLogout(request);

            Assert.True(success);
            Assert.Equal(LoginType.NONE, login);
        }

        [Fact]
        public void Logout_AsPlanner_LogoutSuccess()
        {
            controller.ActiveLogin = LoginType.PLANNER;
            var request = new LogoutRequest(); 

            var (success, _, login) = controller.ProcessLogout(request);

            Assert.True(success);
            Assert.Equal(LoginType.NONE, login);
        }

        [Fact]
        public void Logout_NotLoggedIn_LogoutSuccess()
        {
            controller.ActiveLogin = LoginType.NONE;
            var request = new LogoutRequest(); 

            var (success, _, _) = controller.ProcessLogout(request);

            Assert.False(success);
        }
    }
}
