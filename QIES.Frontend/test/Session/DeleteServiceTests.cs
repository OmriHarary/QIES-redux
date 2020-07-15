using QIES.Frontend.Transaction;
using Xunit;

namespace QIES.Frontend.Session.Tests
{
    public class DeleteServiceTests
    {
        private SessionController controller;

        public DeleteServiceTests()
        {
            controller = new SessionController();
        }

        [Fact]
        public void DeleteService_AsPlanner_DeletionSuccess()
        {
            var validServiceNum = "11111";
            var validServiceName = "Service Name";
            controller.ServicesList.AddService(validServiceNum);
            controller.ActiveLogin = LoginType.PLANNER;
            var request = new DeleteServiceRequest(validServiceNum, validServiceName);

            var (success, _) = controller.ProcessDeleteService(request);

            Assert.True(success);
        }

        [Fact]
        public void DeleteService_AsAgent_DeletionFailure()
        {
            var validServiceNum = "11111";
            var validServiceName = "Service Name";
            controller.ServicesList.AddService(validServiceNum);
            controller.ActiveLogin = LoginType.AGENT;
            var request = new DeleteServiceRequest(validServiceNum, validServiceName);

            var (success, _) = controller.ProcessDeleteService(request);

            Assert.False(success);
        }

        [Fact]
        public void DeleteService_NotLoggedIn_DeletionFailure()
        {
            var validServiceNum = "11111";
            var validServiceName = "Service Name";
            controller.ServicesList.AddService(validServiceNum);
            controller.ActiveLogin = LoginType.NONE;
            var request = new DeleteServiceRequest(validServiceNum, validServiceName);

            var (success, _) = controller.ProcessDeleteService(request);

            Assert.False(success);
        }

        [Fact]
        public void DeleteService_ServiceDoesNotExist_DeletionFailure()
        {
            var validMissingServiceNum = "11111";
            var validServiceName = "Service Name";
            controller.ActiveLogin = LoginType.PLANNER;
            var request = new DeleteServiceRequest(validMissingServiceNum, validServiceName);

            var (success, _) = controller.ProcessDeleteService(request);

            Assert.False(success);
        }

        [Fact]
        public void DeleteService_AttemptedTransactionOnDeletedService_Failure()
        {
            var validServiceNum = "11111";
            var validServiceName = "Service Name";
            controller.ServicesList.AddService(validServiceNum);
            controller.ActiveLogin = LoginType.PLANNER;
            var deleteRequest = new DeleteServiceRequest(validServiceNum, validServiceName);

            var (deleteSuccess, _) = controller.ProcessDeleteService(deleteRequest);

            var validNumberTickets = 1;
            var sellRequest = new SellTicketRequest(validServiceNum, validNumberTickets);

            var (sellSuccess, _) = controller.ProcessSellTicket(sellRequest);

            Assert.True(deleteSuccess);
            Assert.False(sellSuccess);
        }

        [Fact]
        public void DeleteService_BadRequest_DeletionFailure()
        {
            var validServiceNum = "11111";
            var validServiceName = "SN";
            controller.ServicesList.AddService(validServiceNum);
            controller.ActiveLogin = LoginType.AGENT;
            var request = new DeleteServiceRequest(validServiceNum, validServiceName);

            var (success, _) = controller.ProcessDeleteService(request);

            Assert.False(success);
        }
    }
}
