using System.IO;
using Microsoft.Extensions.Logging;
using Moq;
using QIES.Backoffice.Parser.Files;
using QIES.Common;
using QIES.Common.Records;
using Xunit;

namespace QIES.Backoffice.Parser.Tests
{
    public class TransactionSummaryParserTests
    {
        [Fact]
        public void TryParse_AnyFileIOException_FailedParse()
        {
            // Arrange
            var logger = new Mock<ILogger<TransactionSummaryParser>>();
            var tsFile = new Mock<IParserInputFile>();
            tsFile.Setup(csFile => csFile.ReadAllLines())
                .Throws<IOException>();

            var parser = new TransactionSummaryParser(logger.Object);

            // Act
            var result = parser.TryParseFile(tsFile.Object, new ());

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TryParse_SellTicketsTransaction_SucceededParse()
        {
            // Arrange
            var logger = new Mock<ILogger<TransactionSummaryParser>>();
            var tsFile = new Mock<IParserInputFile>();
            tsFile.Setup(csFile => csFile.ReadAllLines())
                .Returns(new string[] { "SEL 11111 12 00000 **** 0" });

            var parser = new TransactionSummaryParser(logger.Object);
            var output = new TransactionQueue();

            // Act
            var result = parser.TryParseFile(tsFile.Object, output);
            var transaction = output.Pop();

            // Assert
            var expected = new TransactionRecord(TransactionCode.SEL)
            {
                SourceNumber = new ServiceNumber("11111"),
                NumberTickets = new NumberTickets(12)
            };

            Assert.True(result);
            Assert.Equal(expected, transaction);
        }

        [Fact]
        public void TryParse_CancelTicketsTransaction_SucceededParse()
        {
            // Arrange
            var logger = new Mock<ILogger<TransactionSummaryParser>>();
            var tsFile = new Mock<IParserInputFile>();
            tsFile.Setup(csFile => csFile.ReadAllLines())
                .Returns(new string[] { "CAN 11111 4 00000 **** 0" });

            var parser = new TransactionSummaryParser(logger.Object);
            var output = new TransactionQueue();

            // Act
            var result = parser.TryParseFile(tsFile.Object, output);
            var transaction = output.Pop();

            // Assert
            var expected = new TransactionRecord(TransactionCode.CAN)
            {
                SourceNumber = new ServiceNumber("11111"),
                NumberTickets = new NumberTickets(4)
            };

            Assert.True(result);
            Assert.Equal(expected, transaction);
        }

        [Fact]
        public void TryParse_DeleteServiceTransaction_SucceededParse()
        {
            // Arrange
            var logger = new Mock<ILogger<TransactionSummaryParser>>();
            var tsFile = new Mock<IParserInputFile>();
            tsFile.Setup(csFile => csFile.ReadAllLines())
                .Returns(new string[] { "DEL 90000 0 00000 sdf 0" });

            var parser = new TransactionSummaryParser(logger.Object);
            var output = new TransactionQueue();

            // Act
            var result = parser.TryParseFile(tsFile.Object, output);
            var transaction = output.Pop();

            // Assert
            var expected = new TransactionRecord(TransactionCode.DEL)
            {
                SourceNumber = new ServiceNumber("90000"),
                ServiceName = new ServiceName("sdf")
            };

            Assert.True(result);
            Assert.Equal(expected, transaction);
        }

        [Fact]
        public void TryParse_CreateServiceTransaction_SucceededParse()
        {
            // Arrange
            var logger = new Mock<ILogger<TransactionSummaryParser>>();
            var tsFile = new Mock<IParserInputFile>();
            tsFile.Setup(csFile => csFile.ReadAllLines())
                .Returns(new string[] { "CRE 12312 0 00000 this is the name 20181212" });

            var parser = new TransactionSummaryParser(logger.Object);
            var output = new TransactionQueue();

            // Act
            var result = parser.TryParseFile(tsFile.Object, output);
            var transaction = output.Pop();

            // Assert
            var expected = new TransactionRecord(TransactionCode.CRE)
            {
                SourceNumber = new ServiceNumber("12312"),
                ServiceName = new ServiceName("this is the name"),
                ServiceDate = new ServiceDate("20181212")
            };

            Assert.True(result);
            Assert.Equal(expected, transaction);
        }

        [Fact]
        public void TryParse_ChangeTicketsTransaction_SucceededParse()
        {
            // Arrange
            var logger = new Mock<ILogger<TransactionSummaryParser>>();
            var tsFile = new Mock<IParserInputFile>();
            tsFile.Setup(csFile => csFile.ReadAllLines())
                .Returns(new string[] { "CHG 11111 7 22222 **** 0" });

            var parser = new TransactionSummaryParser(logger.Object);
            var output = new TransactionQueue();

            // Act
            var result = parser.TryParseFile(tsFile.Object, output);
            var transaction = output.Pop();

            // Assert
            var expected = new TransactionRecord(TransactionCode.CHG)
            {
                SourceNumber = new ServiceNumber("11111"),
                NumberTickets = new NumberTickets(7),
                DestinationNumber = new ServiceNumber("22222")
            };

            Assert.True(result);
            Assert.Equal(expected, transaction);
        }

        [Fact]
        public void TryParse_EndOfSessionTransaction_SucceededParse()
        {
            // Arrange
            var logger = new Mock<ILogger<TransactionSummaryParser>>();
            var tsFile = new Mock<IParserInputFile>();
            tsFile.Setup(csFile => csFile.ReadAllLines())
                .Returns(new string[] { "EOS 00000 0 00000 **** 0" });

            var parser = new TransactionSummaryParser(logger.Object);
            var output = new TransactionQueue();

            // Act
            var result = parser.TryParseFile(tsFile.Object, output);
            var transaction = output.Pop();

            // Assert
            var expected = new TransactionRecord(TransactionCode.EOS);

            Assert.True(result);
            Assert.Equal(expected, transaction);
        }
    }
}
