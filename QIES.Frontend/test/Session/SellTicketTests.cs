using QIES.Frontend.Transaction;
using Xunit;

namespace QIES.Frontend.Session.Tests
{
    public class SellTicketTests
    {
        private SessionController controller;

        public SellTicketTests()
        {
            controller = new SessionController();
        }

        [Fact]
        public void SellTicket_AsPlanner_SaleSuccess()
        {
            var validNumberTickets = 1;
            var validServiceNum = "11111";
            controller.ServicesList.AddService(validServiceNum);
            controller.ActiveLogin = LoginType.PLANNER;
            var request = new SellTicketRequest(validServiceNum, validNumberTickets);

            var (success, _) = controller.ProcessSellTicket(request);

            Assert.True(success);
        }

        [Fact]
        public void SellTicket_AsAgent_SaleSuccess()
        {
            var validNumberTickets = 1;
            var validServiceNum = "11111";
            controller.ServicesList.AddService(validServiceNum);
            controller.ActiveLogin = LoginType.AGENT;
            var request = new SellTicketRequest(validServiceNum, validNumberTickets);

            var (success, _) = controller.ProcessSellTicket(request);

            Assert.True(success);
        }

        [Fact]
        public void SellTicket_NotLoggedIn_SaleFailure()
        {
            var validNumberTickets = 1;
            var validServiceNum = "11111";
            controller.ServicesList.AddService(validServiceNum);
            controller.ActiveLogin = LoginType.NONE;
            var request = new SellTicketRequest(validServiceNum, validNumberTickets);

            var (success, _) = controller.ProcessSellTicket(request);

            Assert.False(success);
        }

        [Fact]
        public void SellTicket_ServiceDoesNotExist_SaleFailure()
        {
            var validNumberTickets = 1;
            var validMissingServiceNum = "11111";
            controller.ActiveLogin = LoginType.AGENT;
            var request = new SellTicketRequest(validMissingServiceNum, validNumberTickets);

            var (success, _) = controller.ProcessSellTicket(request);

            Assert.False(success);
        }

        [Fact]
        public void SellTicket_BadRequest_SaleFailure()
        {
            var validNumberTickets = 0;
            var validServiceNum = "11111";
            controller.ServicesList.AddService(validServiceNum);
            controller.ActiveLogin = LoginType.AGENT;
            var request = new SellTicketRequest(validServiceNum, validNumberTickets);

            var (success, _) = controller.ProcessSellTicket(request);

            Assert.False(success);
        }
    }
}
