using Xunit;
using QIES.Common.Record;

namespace QIES.Common.Tests.Record
{
    public class ServiceDateTests
    {
        [Fact]
        public void IsValid_LengthLowerLimit_Invalid()
        {
            var date = "2000101";

            var valid = ServiceDate.IsValid(date);

            Assert.False(valid);
        }

        [Fact]
        public void IsValid_LengthUpperLimit_Invalid()
        {
            var date = "200010100";

            var valid = ServiceDate.IsValid(date);

            Assert.False(valid);
        }

        [Fact]
        public void IsValid_Nonnumeric_Invalid()
        {
            var date = "2000101x";

            var valid = ServiceDate.IsValid(date);

            Assert.False(valid);
        }

        [Fact]
        public void IsValid_MaxYear_Valid()
        {
            var date = "29991010";

            var valid = ServiceDate.IsValid(date);

            Assert.True(valid);
        }

        [Fact]
        public void IsValid_MaxYear_Invalid()
        {
            var date = "30001010";

            var valid = ServiceDate.IsValid(date);

            Assert.False(valid);
        }

        [Fact]
        public void IsValid_MinYear_Valid()
        {
            var date = "19801010";

            var valid = ServiceDate.IsValid(date);

            Assert.True(valid);
        }

        [Fact]
        public void IsValid_MinYear_Invalid()
        {
            var date = "19791010";

            var valid = ServiceDate.IsValid(date);

            Assert.False(valid);
        }

        [Fact]
        public void IsValid_MaxMonth_Valid()
        {
            var date = "20001210";

            var valid = ServiceDate.IsValid(date);

            Assert.True(valid);
        }

        [Fact]
        public void IsValid_MaxMonth_Invalid()
        {
            var date = "20001310";

            var valid = ServiceDate.IsValid(date);

            Assert.False(valid);
        }

        [Fact]
        public void IsValid_MinMonth_Valid()
        {
            var date = "20000110";

            var valid = ServiceDate.IsValid(date);

            Assert.True(valid);
        }

        [Fact]
        public void IsValid_MinMonth_Invalid()
        {
            var date = "20000010";

            var valid = ServiceDate.IsValid(date);

            Assert.False(valid);
        }

        [Fact]
        public void IsValid_MaxDay_Valid()
        {
            var date = "20001031";

            var valid = ServiceDate.IsValid(date);

            Assert.True(valid);
        }

        [Fact]
        public void IsValid_MaxDay_Invalid()
        {
            var date = "20001032";

            var valid = ServiceDate.IsValid(date);

            Assert.False(valid);
        }

        [Fact]
        public void IsValid_MinDay_Valid()
        {
            var date = "20001001";

            var valid = ServiceDate.IsValid(date);

            Assert.True(valid);
        }

        [Fact]
        public void IsValid_MinDay_Invalid()
        {
            var date = "20001000";

            var valid = ServiceDate.IsValid(date);

            Assert.False(valid);
        }

        [Fact]
        public void ToString_Unset_DefaultValue()
        {
            const string ExpectedDefault = "0";
            var serviceDate = ServiceDate.Empty;

            var tostring = serviceDate.ToString();

            Assert.Equal(ExpectedDefault, tostring);
        }
    }
}
