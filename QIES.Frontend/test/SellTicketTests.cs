using System;
using QIES.Frontend.Session;
using QIES.Frontend.Transaction;
using Xunit;

namespace QIES.Frontend.Tests
{
    public class SellTicketTests
    {
        private SessionController controller;

        public SellTicketTests()
        {
            controller = new SessionController("summary.file");
        }

        [Fact]
        public void SellTicket_AsPlanner_SaleSuccess()
        {
            var numberTickets = 1;
            var validServiceNum = "11111";
            controller.ServicesList.AddService(validServiceNum);
            controller.ActiveLogin = LoginType.PLANNER;
            var request = new SellTicketRequest(validServiceNum, numberTickets);

            var (success, _) = controller.ProcessSellTicket(request);

            Assert.True(success);
        }

        [Fact]
        public void SellTicket_AsAgent_SaleSuccess()
        {
            var numberTickets = 1;
            var validServiceNum = "11111";
            controller.ServicesList.AddService(validServiceNum);
            controller.ActiveLogin = LoginType.AGENT;
            var request = new SellTicketRequest(validServiceNum, numberTickets);

            var (success, _) = controller.ProcessSellTicket(request);

            Assert.True(success);
        }

        [Fact]
        public void SellTicket_NotLoggedIn_SaleFailure()
        {
            var numberTickets = 1;
            var validServiceNum = "11111";
            controller.ServicesList.AddService(validServiceNum);
            controller.ActiveLogin = LoginType.NONE;
            var request = new SellTicketRequest(validServiceNum, numberTickets);

            var (success, _) = controller.ProcessSellTicket(request);

            Assert.False(success);
        }

        [Fact]
        public void SellTicket_InvalidService_SaleFailure()
        {
            var numberTickets = 1;
            var inValidServiceNum = "11111";
            controller.ActiveLogin = LoginType.AGENT;
            var request = new SellTicketRequest(inValidServiceNum, numberTickets);

            var (success, _) = controller.ProcessSellTicket(request);

            Assert.False(success);
        }
    }
}
