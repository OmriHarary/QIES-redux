using System;
using QIES.Frontend.Session;
using QIES.Frontend.Transaction;
using Xunit;

namespace QIES.Frontend.Tests
{
    public class ChangeTicketTests
    {
        private SessionController controller;

        public ChangeTicketTests()
        {
            controller = new SessionController("summary.file");
        }

        [Fact]
        public void ChangeTicket_AsPlanner_ChangeSuccess()
        {
            var numberTickets = 1;
            var validSourceNum = "11111";
            var validDestNum = "11112";
            controller.ServicesList.AddService(validSourceNum);
            controller.ServicesList.AddService(validDestNum);
            controller.ActiveLogin = LoginType.PLANNER;
            var request = new ChangeTicketRequest(validSourceNum, numberTickets, validDestNum);

            var (success, _) = controller.ProcessChangeTicket(request);

            Assert.True(success);
        }

        [Fact]
        public void ChangeTicket_AsAgent_ChangeSuccess()
        {
            var numberTickets = 1;
            var validSourceNum = "11111";
            var validDestNum = "11112";
            controller.ServicesList.AddService(validSourceNum);
            controller.ServicesList.AddService(validDestNum);
            controller.ActiveLogin = LoginType.AGENT;
            var request = new ChangeTicketRequest(validSourceNum, numberTickets, validDestNum);

            var (success, _) = controller.ProcessChangeTicket(request);

            Assert.True(success);
        }

        [Fact]
        public void ChangeTicket_NotLoggedIn_ChangeFailure()
        {
            var numberTickets = 1;
            var validSourceNum = "11111";
            var validDestNum = "11112";
            controller.ServicesList.AddService(validSourceNum);
            controller.ServicesList.AddService(validDestNum);
            controller.ActiveLogin = LoginType.NONE;
            var request = new ChangeTicketRequest(validSourceNum, numberTickets, validDestNum);

            var (success, _) = controller.ProcessChangeTicket(request);

            Assert.False(success);
        }

        [Fact]
        public void ChangeTicket_InvalidSourceService_ChangeFailure()
        {
            var numberTickets = 1;
            var invalidSourceNum = "11111";
            var validDestNum = "11112";
            controller.ServicesList.AddService(validDestNum);
            controller.ActiveLogin = LoginType.AGENT;
            var request = new ChangeTicketRequest(invalidSourceNum, numberTickets, validDestNum);

            var (success, _) = controller.ProcessChangeTicket(request);

            Assert.False(success);
        }

        [Fact]
        public void ChangeTicket_InvalidDestinationService_ChangeFailure()
        {
            var numberTickets = 1;
            var validSourceNum = "11111";
            var invalidDestNum = "11112";
            controller.ServicesList.AddService(validSourceNum);
            controller.ActiveLogin = LoginType.AGENT;
            var request = new ChangeTicketRequest(validSourceNum, numberTickets, invalidDestNum);

            var (success, _) = controller.ProcessChangeTicket(request);

            Assert.False(success);
        }

        [Fact]
        public void ChangeTicket_AgentServiceLimitExceededSingleAsAgent_ChangeFailure()
        {
            var numberTickets = 21;
            var validSourceNum = "11111";
            var validDestNum = "11112";
            controller.ServicesList.AddService(validSourceNum);
            controller.ServicesList.AddService(validDestNum);
            controller.ActiveLogin = LoginType.AGENT;
            var request = new ChangeTicketRequest(validSourceNum, numberTickets, validDestNum);

            var (success, _) = controller.ProcessChangeTicket(request);

            Assert.False(success);
        }

        [Fact]
        public void ChangeTicket_AgentServiceLimitExceededMultipleAsAgent_ChangeFailure()
        {
            var numberTickets1 = 10;
            var numberTickets2 = 11;
            var validSourceNum1 = "11111";
            var validDestNum1 = "11112";
            var validSourceNum2 = "11113";
            var validDestNum2 = "11114";
            controller.ServicesList.AddService(validSourceNum1);
            controller.ServicesList.AddService(validDestNum1);
            controller.ServicesList.AddService(validSourceNum2);
            controller.ServicesList.AddService(validDestNum2);
            controller.ActiveLogin = LoginType.AGENT;
            var request1 = new ChangeTicketRequest(validSourceNum1, numberTickets1, validDestNum1);
            var request2 = new ChangeTicketRequest(validSourceNum2, numberTickets2, validDestNum2);

            _ = controller.ProcessChangeTicket(request2);
            var (success, _) = controller.ProcessChangeTicket(request2);

            Assert.False(success);
        }

        [Fact]
        public void ChangeTicket_AgentServiceLimitExceededSingleAsPlanner_ChangeSuccess()
        {
            var numberTickets = 21;
            var validSourceNum = "11111";
            var validDestNum = "11112";
            controller.ServicesList.AddService(validSourceNum);
            controller.ServicesList.AddService(validDestNum);
            controller.ActiveLogin = LoginType.PLANNER;
            var request = new ChangeTicketRequest(validSourceNum, numberTickets, validDestNum);

            var (success, _) = controller.ProcessChangeTicket(request);

            Assert.True(success);
        }

        [Fact]
        public void ChangeTicket_AgentServiceLimitExceededMultipleAsPlanner_ChangeSuccess()
        {
            var numberTickets1 = 10;
            var numberTickets2 = 11;
            var validSourceNum1 = "11111";
            var validDestNum1 = "11112";
            var validSourceNum2 = "11113";
            var validDestNum2 = "11114";
            controller.ServicesList.AddService(validSourceNum1);
            controller.ServicesList.AddService(validDestNum1);
            controller.ServicesList.AddService(validSourceNum2);
            controller.ServicesList.AddService(validDestNum2);
            controller.ActiveLogin = LoginType.PLANNER;
            var request1 = new ChangeTicketRequest(validSourceNum1, numberTickets1, validDestNum1);
            var request2 = new ChangeTicketRequest(validSourceNum2, numberTickets2, validDestNum2);

            _ = controller.ProcessChangeTicket(request2);
            var (success, _) = controller.ProcessChangeTicket(request2);

            Assert.True(success);
        }
    }
}
