using Xunit;

namespace QIES.Frontend.Transaction.Tests
{
    public class SellTicketTransactionTests
    {
        [Fact]
        public void MakeTransaction_ValidRequest_CreatesCorrectTransactionRecord()
        {
            var validNumberTickets = 2;
            var validServiceNum = "11111";
            var request = new SellTicketRequest(validServiceNum, validNumberTickets);

            var (record, _) = SellTicket.MakeTransaction(request);

            const string ExpectedRecord = "SEL 11111 2 00000 **** 0";
            Assert.Equal(ExpectedRecord, record.ToString());
        }

        [Fact]
        public void MakeTransaction_InvalidServiceNumber_CreatesNoTransactionRecord()
        {
            var validNumberTickets = 1;
            var invalidServiceNum = "01111";
            var request = new SellTicketRequest(invalidServiceNum, validNumberTickets);

            var (record, _) = SellTicket.MakeTransaction(request);

            Assert.Null(record);
        }

        [Fact]
        public void MakeTransaction_InvalidNumberTickets_CreatesNoTransactionRecord()
        {
            var invalidNumberTickets = 0;
            var validServiceNum = "11111";
            var request = new SellTicketRequest(validServiceNum, invalidNumberTickets);

            var (record, _) = SellTicket.MakeTransaction(request);

            Assert.Null(record);
        }
    }
}