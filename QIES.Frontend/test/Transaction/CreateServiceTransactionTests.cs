using Xunit;

namespace QIES.Frontend.Transaction.Tests
{
    public class CreateServiceTransactionTests
    {
        [Fact]
        public void MakeTransaction_ValidRequest_CreatesCorrectTransactionRecord()
        {
            var validServiceNum = "11211";
            var validServiceName = "ANewService";
            var validServiceDate = "20181001";
            var request = new CreateServiceRequest(validServiceNum, validServiceDate, validServiceName);

            var (record, _) = CreateService.MakeTransaction(request);

            const string ExpectedRecord = "CRE 11211 0 00000 ANewService 20181001";
            Assert.Equal(ExpectedRecord, record.ToString());
        }

        [Fact]
        public void MakeTransaction_InvalidServiceNumber_CreatesNoTransactionRecord()
        {
            var invalidServiceNum = "111111";
            var validServiceName = "ANewService";
            var validServiceDate = "20181001";
            var request = new CreateServiceRequest(invalidServiceNum, validServiceDate, validServiceName);

            var (record, _) = CreateService.MakeTransaction(request);

            Assert.Null(record);
        }

        [Fact]
        public void MakeTransaction_InvalidServiceName_CreatesNoTransactionRecord()
        {
            var validServiceNum = "11111";
            var invalidServiceName = "incorrectcreateservice Name Is Way To Long To Work";
            var validServiceDate = "20181001";
            var request = new CreateServiceRequest(validServiceNum, validServiceDate, invalidServiceName);

            var (record, _) = CreateService.MakeTransaction(request);

            Assert.Null(record);
        }

        [Fact]
        public void MakeTransaction_InvalidServiceDate_CreatesNoTransactionRecord()
        {
            var validServiceNum = "11111";
            var validServiceName = "incorrectcreateservice Name Is Way To Long To Work";
            var invalidServiceDate = "30001332";
            var request = new CreateServiceRequest(validServiceNum, invalidServiceDate, validServiceName);

            var (record, _) = CreateService.MakeTransaction(request);

            Assert.Null(record);
        }
    }
}
