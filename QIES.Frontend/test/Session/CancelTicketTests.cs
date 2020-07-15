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
            var validNumberTickets = 1;
            var validServiceNum = "11111";
            controller.ServicesList.AddService(validServiceNum);
            controller.ActiveLogin = LoginType.PLANNER;
            var request = new CancelTicketRequest(validServiceNum, validNumberTickets);

            var (success, _) = controller.ProcessCancelTicket(request);

            Assert.True(success);
        }

        [Fact]
        public void CancelTicket_AsAgent_CancellationSuccess()
        {
            var validNumberTickets = 1;
            var validServiceNum = "11111";
            controller.ServicesList.AddService(validServiceNum);
            controller.ActiveLogin = LoginType.AGENT;
            var request = new CancelTicketRequest(validServiceNum, validNumberTickets);

            var (success, _) = controller.ProcessCancelTicket(request);

            Assert.True(success);
        }

        [Fact]
        public void CancelTicket_NotLoggedIn_CancellationFailure()
        {
            var validNumberTickets = 1;
            var validServiceNum = "11111";
            controller.ServicesList.AddService(validServiceNum);
            controller.ActiveLogin = LoginType.NONE;
            var request = new CancelTicketRequest(validServiceNum, validNumberTickets);

            var (success, _) = controller.ProcessCancelTicket(request);

            Assert.False(success);
        }

        [Fact]
        public void CancelTicket_ServiceDoesNotExist_CancellationFailure()
        {
            var validNumberTickets = 1;
            var validMissingServiceNum = "11111";
            controller.ActiveLogin = LoginType.AGENT;
            var request = new CancelTicketRequest(validMissingServiceNum, validNumberTickets);

            var (success, _) = controller.ProcessCancelTicket(request);

            Assert.False(success);
        }

        [Fact]
        public void CancelTicket_AgentServiceLimitExceededSingleAsAgent_CancellationFailure()
        {
            var validNumberTickets = 11;
            var validServiceNum = "11111";
            controller.ServicesList.AddService(validServiceNum);
            controller.ActiveLogin = LoginType.AGENT;
            var request = new CancelTicketRequest(validServiceNum, validNumberTickets);

            var (success, _) = controller.ProcessCancelTicket(request);

            Assert.False(success);
        }

        [Fact]
        public void CancelTicket_AgentServiceLimitExceededMultipleAsAgent_CancellationFailure()
        {
            var validNumberTickets1 = 8;
            var validNumberTickets2 = 3;
            var validServiceNum = "11111";
            controller.ServicesList.AddService(validServiceNum);
            controller.ActiveLogin = LoginType.AGENT;
            var request1 = new CancelTicketRequest(validServiceNum, validNumberTickets1);
            var request2 = new CancelTicketRequest(validServiceNum, validNumberTickets2);

            _ = controller.ProcessCancelTicket(request1);
            var (success, _) = controller.ProcessCancelTicket(request2);

            Assert.False(success);
        }

        [Fact]
        public void CancelTicket_AgentSessionLimitExceededAsAgent_CancellationFailure()
        {
            var validNumberTickets1 = 8;
            var validNumberTickets2 = 8;
            var validNumberTickets3 = 5;
            var validServiceNum1 = "11111";
            var validServiceNum2 = "11112";
            var validServiceNum3 = "11113";
            controller.ServicesList.AddService(validServiceNum1);
            controller.ServicesList.AddService(validServiceNum2);
            controller.ServicesList.AddService(validServiceNum3);
            controller.ActiveLogin = LoginType.AGENT;
            var request1 = new CancelTicketRequest(validServiceNum1, validNumberTickets1);
            var request2 = new CancelTicketRequest(validServiceNum2, validNumberTickets2);
            var request3 = new CancelTicketRequest(validServiceNum3, validNumberTickets3);

            _ = controller.ProcessCancelTicket(request1);
            _ = controller.ProcessCancelTicket(request2);
            var (success, _) = controller.ProcessCancelTicket(request3);

            Assert.False(success);
        }

        [Fact]
        public void CancelTicket_AgentServiceLimitExceededSingleAsPlanner_CancellationSuccess()
        {
            var validNumberTickets = 11;
            var validServiceNum = "11111";
            controller.ServicesList.AddService(validServiceNum);
            controller.ActiveLogin = LoginType.PLANNER;
            var request = new CancelTicketRequest(validServiceNum, validNumberTickets);

            var (success, _) = controller.ProcessCancelTicket(request);

            Assert.True(success);
        }

        [Fact]
        public void CancelTicket_AgentServiceLimitExceededMultipleAsPlanner_CancellationSuccess()
        {
            var validNumberTickets1 = 8;
            var validNumberTickets2 = 3;
            var validServiceNum = "11111";
            controller.ServicesList.AddService(validServiceNum);
            controller.ActiveLogin = LoginType.PLANNER;
            var request1 = new CancelTicketRequest(validServiceNum, validNumberTickets1);
            var request2 = new CancelTicketRequest(validServiceNum, validNumberTickets2);

            _ = controller.ProcessCancelTicket(request1);
            var (success, _) = controller.ProcessCancelTicket(request2);

            Assert.True(success);
        }

        [Fact]
        public void CancelTicket_AgentSessionLimitExceededAsPlanner_CancellationSuccess()
        {
            var validNumberTickets1 = 8;
            var validNumberTickets2 = 8;
            var validNumberTickets3 = 5;
            var validServiceNum1 = "11111";
            var validServiceNum2 = "11112";
            var validServiceNum3 = "11113";
            controller.ServicesList.AddService(validServiceNum1);
            controller.ServicesList.AddService(validServiceNum2);
            controller.ServicesList.AddService(validServiceNum3);
            controller.ActiveLogin = LoginType.PLANNER;
            var request1 = new CancelTicketRequest(validServiceNum1, validNumberTickets1);
            var request2 = new CancelTicketRequest(validServiceNum2, validNumberTickets2);
            var request3 = new CancelTicketRequest(validServiceNum3, validNumberTickets3);

            _ = controller.ProcessCancelTicket(request1);
            _ = controller.ProcessCancelTicket(request2);
            var (success, _) = controller.ProcessCancelTicket(request3);

            Assert.True(success);
        }

        [Fact]
        public void CancelTicket_BadRequest_CancellationFailure()
        {
            var validNumberTickets = 0;
            var validServiceNum = "11111";
            controller.ServicesList.AddService(validServiceNum);
            controller.ActiveLogin = LoginType.AGENT;
            var request = new CancelTicketRequest(validServiceNum, validNumberTickets);

            var (success, _) = controller.ProcessCancelTicket(request);

            Assert.False(success);
        }
    }
}
