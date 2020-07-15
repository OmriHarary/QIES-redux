using QIES.Frontend.Session;
using QIES.Frontend.Transaction;
using Xunit;

namespace QIES.Frontend.Session.Tests
{
    public class CancelTicketTests
    {
        private SessionController controller;

        public CancelTicketTests()
        {
            controller = new SessionController();
        }

        [Fact]
        public void CancelTicket_AsPlanner_CancellationSuccess()
        {
            var numberTickets = 1;
            var validServiceNum = "11111";
            controller.ServicesList.AddService(validServiceNum);
            controller.ActiveLogin = LoginType.PLANNER;
            var request = new CancelTicketRequest(validServiceNum, numberTickets);

            var (success, _) = controller.ProcessCancelTicket(request);

            Assert.True(success);
        }

        [Fact]
        public void CancelTicket_AsAgent_CancellationSuccess()
        {
            var numberTickets = 1;
            var validServiceNum = "11111";
            controller.ServicesList.AddService(validServiceNum);
            controller.ActiveLogin = LoginType.AGENT;
            var request = new CancelTicketRequest(validServiceNum, numberTickets);

            var (success, _) = controller.ProcessCancelTicket(request);

            Assert.True(success);
        }

        [Fact]
        public void CancelTicket_NotLoggedIn_CancellationFailure()
        {
            var numberTickets = 1;
            var validServiceNum = "11111";
            controller.ServicesList.AddService(validServiceNum);
            controller.ActiveLogin = LoginType.NONE;
            var request = new CancelTicketRequest(validServiceNum, numberTickets);

            var (success, _) = controller.ProcessCancelTicket(request);

            Assert.False(success);
        }

        [Fact]
        public void CancelTicket_InvalidService_CancellationFailure()
        {
            var numberTickets = 1;
            var inValidServiceNum = "11111";
            controller.ActiveLogin = LoginType.AGENT;
            var request = new CancelTicketRequest(inValidServiceNum, numberTickets);

            var (success, _) = controller.ProcessCancelTicket(request);

            Assert.False(success);
        }

        [Fact]
        public void CancelTicket_AgentServiceLimitExceededSingleAsAgent_CancellationFailure()
        {
            var numberTickets = 11;
            var validServiceNum = "11111";
            controller.ServicesList.AddService(validServiceNum);
            controller.ActiveLogin = LoginType.AGENT;
            var request = new CancelTicketRequest(validServiceNum, numberTickets);

            var (success, _) = controller.ProcessCancelTicket(request);

            Assert.False(success);
        }

        [Fact]
        public void CancelTicket_AgentServiceLimitExceededMultipleAsAgent_CancellationFailure()
        {
            var numberTickets1 = 8;
            var numberTickets2 = 3;
            var validServiceNum = "11111";
            controller.ServicesList.AddService(validServiceNum);
            controller.ActiveLogin = LoginType.AGENT;
            var request1 = new CancelTicketRequest(validServiceNum, numberTickets1);
            var request2 = new CancelTicketRequest(validServiceNum, numberTickets2);

            _ = controller.ProcessCancelTicket(request1);
            var (success, _) = controller.ProcessCancelTicket(request2);

            Assert.False(success);
        }

        [Fact]
        public void CancelTicket_AgentSessionLimitExceededAsAgent_CancellationFailure()
        {
            var numberTickets1 = 8;
            var numberTickets2 = 8;
            var numberTickets3 = 5;
            var validServiceNum1 = "11111";
            var validServiceNum2 = "11112";
            var validServiceNum3 = "11113";
            controller.ServicesList.AddService(validServiceNum1);
            controller.ServicesList.AddService(validServiceNum2);
            controller.ServicesList.AddService(validServiceNum3);
            controller.ActiveLogin = LoginType.AGENT;
            var request1 = new CancelTicketRequest(validServiceNum1, numberTickets1);
            var request2 = new CancelTicketRequest(validServiceNum2, numberTickets2);
            var request3 = new CancelTicketRequest(validServiceNum3, numberTickets3);

            _ = controller.ProcessCancelTicket(request1);
            _ = controller.ProcessCancelTicket(request2);
            var (success, _) = controller.ProcessCancelTicket(request3);

            Assert.False(success);
        }

        [Fact]
        public void CancelTicket_AgentServiceLimitExceededSingleAsPlanner_CancellationSuccess()
        {
            var numberTickets = 11;
            var validServiceNum = "11111";
            controller.ServicesList.AddService(validServiceNum);
            controller.ActiveLogin = LoginType.PLANNER;
            var request = new CancelTicketRequest(validServiceNum, numberTickets);

            var (success, _) = controller.ProcessCancelTicket(request);

            Assert.True(success);
        }

        [Fact]
        public void CancelTicket_AgentServiceLimitExceededMultipleAsPlanner_CancellationSuccess()
        {
            var numberTickets1 = 8;
            var numberTickets2 = 3;
            var validServiceNum = "11111";
            controller.ServicesList.AddService(validServiceNum);
            controller.ActiveLogin = LoginType.PLANNER;
            var request1 = new CancelTicketRequest(validServiceNum, numberTickets1);
            var request2 = new CancelTicketRequest(validServiceNum, numberTickets2);

            _ = controller.ProcessCancelTicket(request1);
            var (success, _) = controller.ProcessCancelTicket(request2);

            Assert.True(success);
        }

        [Fact]
        public void CancelTicket_AgentSessionLimitExceededAsPlanner_CancellationSuccess()
        {
            var numberTickets1 = 8;
            var numberTickets2 = 8;
            var numberTickets3 = 5;
            var validServiceNum1 = "11111";
            var validServiceNum2 = "11112";
            var validServiceNum3 = "11113";
            controller.ServicesList.AddService(validServiceNum1);
            controller.ServicesList.AddService(validServiceNum2);
            controller.ServicesList.AddService(validServiceNum3);
            controller.ActiveLogin = LoginType.PLANNER;
            var request1 = new CancelTicketRequest(validServiceNum1, numberTickets1);
            var request2 = new CancelTicketRequest(validServiceNum2, numberTickets2);
            var request3 = new CancelTicketRequest(validServiceNum3, numberTickets3);

            _ = controller.ProcessCancelTicket(request1);
            _ = controller.ProcessCancelTicket(request2);
            var (success, _) = controller.ProcessCancelTicket(request3);

            Assert.True(success);
        }
    }
}
