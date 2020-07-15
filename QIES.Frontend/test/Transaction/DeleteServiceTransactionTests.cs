using Xunit;

namespace QIES.Frontend.Transaction.Tests
{
    public class DeleteServiceTransactionTests
    {
        [Fact]
        public void MakeTransaction_ValidRequest_CreatesCorrectTransactionRecord()
        {
            var validServiceNum = "11111";
            var validServiceName = "Testcreateservice";
            var request = new DeleteServiceRequest(validServiceNum, validServiceName);

            var (record, _) = DeleteService.MakeTransaction(request);

            const string ExpectedRecord = "DEL 11111 0 00000 Testcreateservice 0";
            Assert.Equal(ExpectedRecord, record.ToString());
        }

        [Fact]
        public void MakeTransaction_InvalidServiceNumber_CreatesNoTransactionRecord()
        {
            var invalidServiceNum = "1111";
            var validServiceName = "ANewService";
            var request = new DeleteServiceRequest(invalidServiceNum, validServiceName);

            var (record, _) = DeleteService.MakeTransaction(request);

            Assert.Null(record);
        }

        [Fact]
        public void MakeTransaction_InvalidServiceName_CreatesNoTransactionRecord()
        {
            var validServiceNum = "11111";
            var invalidServiceName = "incorrectcreateservice Name Is Way To Long To Work";
            var request = new DeleteServiceRequest(validServiceNum, invalidServiceName);

            var (record, _) = DeleteService.MakeTransaction(request);

            Assert.Null(record);
        }
    }
}
