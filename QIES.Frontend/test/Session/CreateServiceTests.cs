using QIES.Frontend.Session;
using QIES.Frontend.Transaction;
using Xunit;

namespace QIES.Frontend.Session.Tests
{
    public class CreateServiceTests
    {
        private SessionController controller;

        public CreateServiceTests()
        {
            controller = new SessionController();
        }

        [Fact]
        public void CreateService_AsPlanner_CreationSuccess()
        {
            var newServiceNum = "11111";
            var newServiceName = "New Service";
            var newServiceDate = "20201010";
            controller.ActiveLogin = LoginType.PLANNER;
            var request = new CreateServiceRequest(newServiceNum, newServiceDate, newServiceName);

            var (success, _) = controller.ProcessCreateService(request);

            Assert.True(success);
        }

        [Fact]
        public void CreateService_AsAgent_CreationFailure()
        {
            var newServiceNum = "11111";
            var newServiceName = "New Service";
            var newServiceDate = "20201010";
            controller.ActiveLogin = LoginType.AGENT;
            var request = new CreateServiceRequest(newServiceNum, newServiceDate, newServiceName);

            var (success, _) = controller.ProcessCreateService(request);

            Assert.False(success);
        }

        [Fact]
        public void CreateService_NotLoggedIn_CreationFailure()
        {
            var newServiceNum = "11111";
            var newServiceName = "New Service";
            var newServiceDate = "20201010";
            controller.ActiveLogin = LoginType.NONE;
            var request = new CreateServiceRequest(newServiceNum, newServiceDate, newServiceName);

            var (success, _) = controller.ProcessCreateService(request);

            Assert.False(success);
        }

        [Fact]
        public void CreateService_ServiceAlreadyExists_CreationFailure()
        {
            var existingServiceNum = "11111";
            var newServiceName = "New Service";
            var newServiceDate = "20201010";
            controller.ServicesList.AddService(existingServiceNum);
            controller.ActiveLogin = LoginType.PLANNER;
            var request = new CreateServiceRequest(existingServiceNum, newServiceDate, newServiceName);

            var (success, _) = controller.ProcessCreateService(request);

            Assert.False(success);
        }

        [Fact]
        public void CreateService_AttemptedTransactionOnNewService_Failure()
        {
            var newServiceNum = "11111";
            var newServiceName = "New Service";
            var newServiceDate = "20201010";
            controller.ActiveLogin = LoginType.PLANNER;
            var createRequest = new CreateServiceRequest(newServiceNum, newServiceDate, newServiceName);

            var (createSuccess, _) = controller.ProcessCreateService(createRequest);

            var numberTickets = 1;
            var sellRequest = new SellTicketRequest(newServiceNum, numberTickets);

            var (sellSuccess, _) = controller.ProcessSellTicket(sellRequest);

            Assert.True(createSuccess);
            Assert.False(sellSuccess);
        }
    }
}
