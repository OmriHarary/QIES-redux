using Xunit;

namespace QIES.Frontend.Transaction.Tests
{
    public class ChangeTicketTransactionTests
    {
        [Fact]
        public void MakeTransaction_ValidRequest_CreatesCorrectTransactionRecord()
        {
            var validNumberTickets = 3;
            var validSourceNum = "11111";
            var validDestNum = "12345";
            var request = new ChangeTicketRequest(validSourceNum, validNumberTickets, validDestNum);

            var (record, _) = ChangeTicket.MakeTransaction(request);

            const string ExpectedRecord = "CHG 11111 3 12345 **** 0";
            Assert.Equal(ExpectedRecord, record.ToString());
        }

        [Fact]
        public void MakeTransaction_InvalidSourceServiceNumber_CreatesNoTransactionRecord()
        {
            var validNumberTickets = 3;
            var invalidSourceNum = "01111";
            var validDestNum = "12345";
            var request = new ChangeTicketRequest(invalidSourceNum, validNumberTickets, validDestNum);

            var (record, _) = ChangeTicket.MakeTransaction(request);

            Assert.Null(record);
        }

        [Fact]
        public void MakeTransaction_InvalidDestinationServiceNumber_CreatesNoTransactionRecord()
        {
            var validNumberTickets = 3;
            var validSourceNum = "11111";
            var invalidDestNum = "1234";
            var request = new ChangeTicketRequest(validSourceNum, validNumberTickets, invalidDestNum);

            var (record, _) = ChangeTicket.MakeTransaction(request);

            Assert.Null(record);
        }

        [Fact]
        public void MakeTransaction_InvalidNumberTickets_CreatesNoTransactionRecord()
        {
            var invalidNumberTickets = 0;
            var validSourceNum = "11111";
            var validDestNum = "12345";
            var request = new ChangeTicketRequest(validSourceNum, invalidNumberTickets, validDestNum);

            var (record, _) = ChangeTicket.MakeTransaction(request);

            Assert.Null(record);
        }
    }
}
