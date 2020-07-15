using Xunit;

namespace QIES.Frontend.Transaction.Tests
{
    public class CancelTicketTransactionTests
    {
        [Fact]
        public void MakeTransaction_ValidRequest_CreatesCorrectTransactionRecord()
        {
            var numberTickets = 1;
            var validServiceNum = "11111";
            var request = new CancelTicketRequest(validServiceNum, numberTickets);

            var (record, _) = CancelTicket.MakeTransaction(request);

            const string ExpectedRecord = "CAN 11111 1 00000 **** 0";
            Assert.Equal(ExpectedRecord, record.ToString());
        }

        [Fact]
        public void MakeTransaction_InvalidServiceNumber_CreatesNoTransactionRecord()
        {
            var numberTickets = 1;
            var invalidServiceNum = "01111";
            var request = new CancelTicketRequest(invalidServiceNum, numberTickets);

            var (record, _) = CancelTicket.MakeTransaction(request);

            Assert.Null(record);
        }

        [Fact]
        public void MakeTransaction_InvalidNumberTickets_CreatesNoTransactionRecord()
        {
            var invalidNumberTickets = 0;
            var validServiceNum = "11111";
            var request = new CancelTicketRequest(validServiceNum, invalidNumberTickets);

            var (record, _) = CancelTicket.MakeTransaction(request);

            Assert.Null(record);
        }
    }
}
