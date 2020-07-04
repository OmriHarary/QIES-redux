using Xunit;
using QIES.Common.Record;

namespace QIES.Common.Tests.Record
{
    public class ServiceNumberTests
    {
        [Fact]
        public void IsValid_LengthLowerLimit_Invalid()
        {
            var number = "1234";

            var valid = ServiceNumber.IsValid(number);

            Assert.False(valid);
        }

        [Fact]
        public void IsValid_LengthUpperLimit_Invalid()
        {
            var number = "123456";

            var valid = ServiceNumber.IsValid(number);

            Assert.False(valid);
        }

        [Fact]
        public void IsValid_StartingZero_Invalid()
        {
            var number = "01234";

            var valid = ServiceNumber.IsValid(number);

            Assert.False(valid);
        }

        [Fact]
        public void ToString_Unset_DefaultValue()
        {
            const string ExpectedDefault = "00000";
            var serviceNumber = new ServiceNumber();

            var tostring = serviceNumber.ToString();

            Assert.Equal(ExpectedDefault, tostring);
        }
    }
}