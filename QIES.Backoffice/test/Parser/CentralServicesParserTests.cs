using System.IO;
using Microsoft.Extensions.Logging;
using Moq;
using QIES.Backoffice.Parser.Files;
using QIES.Common;
using QIES.Common.Records;
using Xunit;

namespace QIES.Backoffice.Parser.Tests
{
    public class CentralServicesParserTests
    {
        [Fact]
        public void TryParseFile_FileNotFound_NewFileCreated()
        {
            // Arrange
            var logger = new Mock<ILogger<CentralServicesParser>>();
            var csFile = new Mock<IParserInputFile>();
            csFile.Setup(csFile => csFile.ReadAllLines())
                .Throws<FileNotFoundException>();

            var parser = new CentralServicesParser(logger.Object);

            // Act
            parser.TryParseFile(csFile.Object, new ());

            // Assert
            csFile.Verify(csFile => csFile.Create());
        }

        [Fact]
        public void TryParseFile_AnyOtherFileIOException_FailedParse()
        {
            // Arrange
            var logger = new Mock<ILogger<CentralServicesParser>>();
            var csFile = new Mock<IParserInputFile>();
            csFile.Setup(csFile => csFile.ReadAllLines())
                .Throws<IOException>();

            var parser = new CentralServicesParser(logger.Object);

            // Act
            var result = parser.TryParseFile(csFile.Object, new ());

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TryParseFile_ValidServiceInput_SucceededParse()
        {
            // Arrange
            var logger = new Mock<ILogger<CentralServicesParser>>();
            var csFile = new Mock<IParserInputFile>();
            csFile.Setup(csFile => csFile.ReadAllLines())
                .Returns(new string[] { "11111 200 0 service1" });

            var parser = new CentralServicesParser(logger.Object);
            var centralServicesList = new CentralServicesList();

            // Act
            var result = parser.TryParseFile(csFile.Object, centralServicesList);

            // Assert
            var expected = new Service
            {
                ServiceNumber = new ServiceNumber("11111"),
                ServiceCapacity = 200,
                ServiceName = new ServiceName("service1")
            };

            Assert.True(result);
            Assert.True(centralServicesList.Contains(expected.ServiceNumber));
            Assert.Equal(expected, centralServicesList.Get(expected.ServiceNumber));
        }

        [Fact]
        public void TryParseFile_InvalidServiceInput_FailedParse()
        {
            // Arrange
            var logger = new Mock<ILogger<CentralServicesParser>>();
            var csFile = new Mock<IParserInputFile>();
            csFile.Setup(csFile => csFile.ReadAllLines())
                .Returns(new string[] { "00010 200 0 s" });

            var parser = new CentralServicesParser(logger.Object);
            var centralServicesList = new CentralServicesList();

            // Act
            var result = parser.TryParseFile(csFile.Object, centralServicesList);

            // Assert
            Assert.False(result);
        }
    }
}
