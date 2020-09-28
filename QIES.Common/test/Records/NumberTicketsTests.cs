using Xunit;

namespace QIES.Common.Records.Tests
{
    public class NumberTicketsTests
    {
        [Fact]
        public void IsValid_LowerLimit_Valid()
        {
            var ticketCount = 1;

            var valid = NumberTickets.IsValid(ticketCount);

            Assert.True(valid);
        }

        [Fact]
        public void IsValid_LowerLimit_Invalid()
        {
            var ticketCount = 0;

            var valid = NumberTickets.IsValid(ticketCount);

            Assert.False(valid);
        }

        [Fact]
        public void IsValid_UpperLimit_Valid()
        {
            var ticketCount = 1000;

            var valid = NumberTickets.IsValid(ticketCount);

            Assert.True(valid);
        }

        [Fact]
        public void IsValid_UpperLimit_Invalid()
        {
            var ticketCount = 1001;

            var valid = NumberTickets.IsValid(ticketCount);

            Assert.False(valid);
        }

        [Fact]
        public void ToString_Unset_DefaultValue()
        {
            const string ExpectedDefault = "0";
            var numberTickets = NumberTickets.Empty;

            var tostring = numberTickets.ToString();

            Assert.Equal(ExpectedDefault, tostring);
        }
    }
}
