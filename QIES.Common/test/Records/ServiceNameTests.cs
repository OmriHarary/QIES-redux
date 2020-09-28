using Xunit;

namespace QIES.Common.Records.Tests
{
    public class ServiceNameTests
    {
        [Fact]
        public void IsValid_LengthLowerLimit_Valid()
        {
            var name = "the";

            var valid = ServiceName.IsValid(name);

            Assert.True(valid);
        }

        [Fact]
        public void IsValid_LengthLowerLimit_Invalid()
        {
            var name = "th";

            var valid = ServiceName.IsValid(name);

            Assert.False(valid);
        }

        [Fact]
        public void IsValid_LengthUpperLimit_Valid()
        {
            var name = "the quick brown fox jumps over the lazy";

            var valid = ServiceName.IsValid(name);

            Assert.True(valid);
        }

        [Fact]
        public void IsValid_LengthUpperLimit_Invalid()
        {
            var name = "the quick brown fox jumps over the lazy dog";

            var valid = ServiceName.IsValid(name);

            Assert.False(valid);
        }

        [Fact]
        public void IsValid_StartingSpace_Invalid()
        {
            var name = " service";

            var valid = ServiceName.IsValid(name);

            Assert.False(valid);
        }

        [Fact]
        public void IsValid_EndingSpace_Invalid()
        {
            var name = "service ";

            var valid = ServiceName.IsValid(name);

            Assert.False(valid);
        }

        [Theory]
        [InlineData("KFLA 67")]
        [InlineData("Gananoque")]
        [InlineData("'''Stra'n''g'e 'ButAllowed'")]
        public void IsValid_SpecExamples_Valid(string name)
        {
            var valid = ServiceName.IsValid(name);

            Assert.True(valid);
        }

        [Fact]
        public void ToString_Unset_DefaultValue()
        {
            const string ExpectedDefault = "****";
            var serviceName = ServiceName.Empty;

            var tostring = serviceName.ToString();

            Assert.Equal(ExpectedDefault, tostring);
        }
    }
}
