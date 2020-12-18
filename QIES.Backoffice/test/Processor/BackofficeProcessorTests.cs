using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using QIES.Backoffice.Config;
using QIES.Backoffice.Parser;
using QIES.Common;
using QIES.Common.Record;
using Xunit;

namespace QIES.Backoffice.Processor.Tests
{
    public class BackofficeProcessorTests
    {
        [Fact]
        public void ProcessCRE_ValidTransaction_Processed()
        {
            // Arrange
            var logger = new Mock<ILogger<BackofficeProcessor>>();
            var options = new Mock<IOptions<ServicesFilesOptions>>();
            var parser = new Mock<IParser<TransactionQueue>>();
            var centralServices = new Mock<ICentralServicesList>();
            centralServices.Setup(centralServices => centralServices.Contains(It.IsAny<ServiceNumber>()))
                .Returns(false);

            var processor = new BackofficeProcessor(logger.Object, options.Object, parser.Object, centralServices.Object);
            var record = new TransactionRecord(TransactionCode.CRE)
            {
                SourceNumber = new ServiceNumber("12312"),
                NumberTickets = new NumberTickets(),
                DestinationNumber = new ServiceNumber(),
                ServiceName = new ServiceName("this is the name"),
                ServiceDate = new ServiceDate("20181212")
            };
        
            // Act
            var result = processor.ProcessCRE(record);
        
            // Assert
            var expectedService = new Service
            {
                ServiceNumber = new ServiceNumber("12312"),
                ServiceName = new ServiceName("this is the name")
            };

            Assert.True(result);
            centralServices.Verify(centralServices => centralServices.Add(expectedService));
        }

        [Fact]
        public void ProcessCRE_ServiceAlreadyExists_NotProcessed()
        {
            // Arrange
            var logger = new Mock<ILogger<BackofficeProcessor>>();
            var options = new Mock<IOptions<ServicesFilesOptions>>();
            var parser = new Mock<IParser<TransactionQueue>>();
            var centralServices = new Mock<ICentralServicesList>();
            centralServices.Setup(centralServices => centralServices.Contains(It.IsAny<ServiceNumber>()))
                .Returns(true);

            var processor = new BackofficeProcessor(logger.Object, options.Object, parser.Object, centralServices.Object);
            var record = new TransactionRecord(TransactionCode.CRE)
            {
                SourceNumber = new ServiceNumber("12312"),
                NumberTickets = new NumberTickets(),
                DestinationNumber = new ServiceNumber(),
                ServiceName = new ServiceName("this is the name"),
                ServiceDate = new ServiceDate("20181212")
            };
        
            // Act
            var result = processor.ProcessCRE(record);
        
            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ProcessDEL_ValidTransaction_Processed()
        {
            // Arrange
            var toDelete = new Service
            {
                ServiceNumber = new ServiceNumber("90000"),
                ServiceName = new ServiceName("sdf")
            };

            var logger = new Mock<ILogger<BackofficeProcessor>>();
            var options = new Mock<IOptions<ServicesFilesOptions>>();
            var parser = new Mock<IParser<TransactionQueue>>();
            var centralServices = new Mock<ICentralServicesList>();
            centralServices.Setup(centralServices => centralServices.Contains(toDelete.ServiceNumber))
                .Returns(true);
            centralServices.Setup(centralServices => centralServices.Get(toDelete.ServiceNumber))
                .Returns(toDelete);

            var processor = new BackofficeProcessor(logger.Object, options.Object, parser.Object, centralServices.Object);
            var record = new TransactionRecord(TransactionCode.DEL)
            {
                SourceNumber = new ServiceNumber("90000"),
                NumberTickets = new NumberTickets(),
                DestinationNumber = new ServiceNumber(),
                ServiceName = new ServiceName("sdf"),
                ServiceDate = new ServiceDate()
            };
        
            // Act
            var result = processor.ProcessDEL(record);
        
            // Assert
            Assert.True(result);
            centralServices.Verify(centralServices => centralServices.Delete(toDelete.ServiceNumber));
        }

        [Fact]
        public void ProcessDEL_ServiceDoesNotExist_NotProcessed()
        {
            // Arrange
            var logger = new Mock<ILogger<BackofficeProcessor>>();
            var options = new Mock<IOptions<ServicesFilesOptions>>();
            var parser = new Mock<IParser<TransactionQueue>>();
            var centralServices = new Mock<ICentralServicesList>();
            centralServices.Setup(centralServices => centralServices.Contains(It.IsAny<ServiceNumber>()))
                .Returns(false);

            var processor = new BackofficeProcessor(logger.Object, options.Object, parser.Object, centralServices.Object);
            var record = new TransactionRecord(TransactionCode.DEL)
            {
                SourceNumber = new ServiceNumber("90000"),
                NumberTickets = new NumberTickets(),
                DestinationNumber = new ServiceNumber(),
                ServiceName = new ServiceName("sdf"),
                ServiceDate = new ServiceDate()
            };
        
            // Act
            var result = processor.ProcessDEL(record);
        
            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ProcessDEL_ServiceHasOutstandingTickets_NotProcessed()
        {
            // Arrange
            var toDelete = new Service
            {
                ServiceNumber = new ServiceNumber("90000"),
                ServiceName = new ServiceName("sdf"),
                TicketsSold = new NumberTickets(1)
            };

            var logger = new Mock<ILogger<BackofficeProcessor>>();
            var options = new Mock<IOptions<ServicesFilesOptions>>();
            var parser = new Mock<IParser<TransactionQueue>>();
            var centralServices = new Mock<ICentralServicesList>();
            centralServices.Setup(centralServices => centralServices.Contains(toDelete.ServiceNumber))
                .Returns(true);
            centralServices.Setup(centralServices => centralServices.Get(toDelete.ServiceNumber))
                .Returns(toDelete);

            var processor = new BackofficeProcessor(logger.Object, options.Object, parser.Object, centralServices.Object);
            var record = new TransactionRecord(TransactionCode.DEL)
            {
                SourceNumber = new ServiceNumber("90000"),
                NumberTickets = new NumberTickets(),
                DestinationNumber = new ServiceNumber(),
                ServiceName = new ServiceName("sdf"),
                ServiceDate = new ServiceDate()
            };
        
            // Act
            var result = processor.ProcessDEL(record);
        
            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ProcessSEL_ValidTransaction_Processed()
        {
            // Arrange
            var service = new Service
            {
                ServiceNumber = new ServiceNumber("11111")
            };

            var logger = new Mock<ILogger<BackofficeProcessor>>();
            var options = new Mock<IOptions<ServicesFilesOptions>>();
            var parser = new Mock<IParser<TransactionQueue>>();
            var centralServices = new Mock<ICentralServicesList>();
            centralServices.Setup(centralServices => centralServices.Contains(service.ServiceNumber))
                .Returns(true);
            centralServices.Setup(centralServices => centralServices.Get(service.ServiceNumber))
                .Returns(service);

            var processor = new BackofficeProcessor(logger.Object, options.Object, parser.Object, centralServices.Object);
            var record = new TransactionRecord(TransactionCode.SEL)
            {
                SourceNumber = new ServiceNumber("11111"),
                NumberTickets = new NumberTickets(12),
                DestinationNumber = new ServiceNumber(),
                ServiceName = new ServiceName(),
                ServiceDate = new ServiceDate()
            };
        
            // Act
            var result = processor.ProcessSEL(record);
        
            // Assert
            Assert.True(result);
            Assert.Equal(new NumberTickets(12), service.TicketsSold);
        }

        [Fact]
        public void ProcessSEL_ServiceDoesNotExist_NotProcessed()
        {
            // Arrange
            var logger = new Mock<ILogger<BackofficeProcessor>>();
            var options = new Mock<IOptions<ServicesFilesOptions>>();
            var parser = new Mock<IParser<TransactionQueue>>();
            var centralServices = new Mock<ICentralServicesList>();
            centralServices.Setup(centralServices => centralServices.Contains(It.IsAny<ServiceNumber>()))
                .Returns(false);

            var processor = new BackofficeProcessor(logger.Object, options.Object, parser.Object, centralServices.Object);
            var record = new TransactionRecord(TransactionCode.SEL)
            {
                SourceNumber = new ServiceNumber("11111"),
                NumberTickets = new NumberTickets(12),
                DestinationNumber = new ServiceNumber(),
                ServiceName = new ServiceName(),
                ServiceDate = new ServiceDate()
            };
        
            // Act
            var result = processor.ProcessSEL(record);
        
            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ProcessSEL_CapacityExceeded_NotProcessed()
        {
            // Arrange
            var service = new Service
            {
                ServiceNumber = new ServiceNumber("11111"),
                ServiceCapacity = 10
            };

            var logger = new Mock<ILogger<BackofficeProcessor>>();
            var options = new Mock<IOptions<ServicesFilesOptions>>();
            var parser = new Mock<IParser<TransactionQueue>>();
            var centralServices = new Mock<ICentralServicesList>();
            centralServices.Setup(centralServices => centralServices.Contains(service.ServiceNumber))
                .Returns(true);
            centralServices.Setup(centralServices => centralServices.Get(service.ServiceNumber))
                .Returns(service);

            var processor = new BackofficeProcessor(logger.Object, options.Object, parser.Object, centralServices.Object);
            var record = new TransactionRecord(TransactionCode.SEL)
            {
                SourceNumber = new ServiceNumber("11111"),
                NumberTickets = new NumberTickets(12),
                DestinationNumber = new ServiceNumber(),
                ServiceName = new ServiceName(),
                ServiceDate = new ServiceDate()
            };
        
            // Act
            var result = processor.ProcessSEL(record);
        
            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ProcessCAN_ValidTransaction_Processed()
        {
            // Arrange
            var service = new Service
            {
                ServiceNumber = new ServiceNumber("11111"),
                TicketsSold = new NumberTickets(5)
            };

            var logger = new Mock<ILogger<BackofficeProcessor>>();
            var options = new Mock<IOptions<ServicesFilesOptions>>();
            var parser = new Mock<IParser<TransactionQueue>>();
            var centralServices = new Mock<ICentralServicesList>();
            centralServices.Setup(centralServices => centralServices.Contains(service.ServiceNumber))
                .Returns(true);
            centralServices.Setup(centralServices => centralServices.Get(service.ServiceNumber))
                .Returns(service);

            var processor = new BackofficeProcessor(logger.Object, options.Object, parser.Object, centralServices.Object);
            var record = new TransactionRecord(TransactionCode.CAN)
            {
                SourceNumber = new ServiceNumber("11111"),
                NumberTickets = new NumberTickets(4),
                DestinationNumber = new ServiceNumber(),
                ServiceName = new ServiceName(),
                ServiceDate = new ServiceDate()
            };
        
            // Act
            var result = processor.ProcessCAN(record);
        
            // Assert
            Assert.True(result);
            Assert.Equal(new NumberTickets(1), service.TicketsSold);
        }

        [Fact]
        public void ProcessCAN_ServiceDoesNotExist_NotProcessed()
        {
            // Arrange
            var logger = new Mock<ILogger<BackofficeProcessor>>();
            var options = new Mock<IOptions<ServicesFilesOptions>>();
            var parser = new Mock<IParser<TransactionQueue>>();
            var centralServices = new Mock<ICentralServicesList>();
            centralServices.Setup(centralServices => centralServices.Contains(It.IsAny<ServiceNumber>()))
                .Returns(false);

            var processor = new BackofficeProcessor(logger.Object, options.Object, parser.Object, centralServices.Object);
            var record = new TransactionRecord(TransactionCode.CAN)
            {
                SourceNumber = new ServiceNumber("11111"),
                NumberTickets = new NumberTickets(4),
                DestinationNumber = new ServiceNumber(),
                ServiceName = new ServiceName(),
                ServiceDate = new ServiceDate()
            };
        
            // Act
            var result = processor.ProcessCAN(record);
        
            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ProcessCAN_TicketsSoldExceeded_NotProcessed()
        {
            // Arrange
            var service = new Service
            {
                ServiceNumber = new ServiceNumber("11111"),
                TicketsSold = new NumberTickets(2)
            };

            var logger = new Mock<ILogger<BackofficeProcessor>>();
            var options = new Mock<IOptions<ServicesFilesOptions>>();
            var parser = new Mock<IParser<TransactionQueue>>();
            var centralServices = new Mock<ICentralServicesList>();
            centralServices.Setup(centralServices => centralServices.Contains(service.ServiceNumber))
                .Returns(true);
            centralServices.Setup(centralServices => centralServices.Get(service.ServiceNumber))
                .Returns(service);

            var processor = new BackofficeProcessor(logger.Object, options.Object, parser.Object, centralServices.Object);
            var record = new TransactionRecord(TransactionCode.CAN)
            {
                SourceNumber = new ServiceNumber("11111"),
                NumberTickets = new NumberTickets(4),
                DestinationNumber = new ServiceNumber(),
                ServiceName = new ServiceName(),
                ServiceDate = new ServiceDate()
            };
        
            // Act
            var result = processor.ProcessCAN(record);
        
            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ProcessCHG_ValidTransaction_Processed()
        {
            // Arrange
            var sourceService = new Service
            {
                ServiceNumber = new ServiceNumber("11111"),
                TicketsSold = new NumberTickets(8)
            };
            var destinationService = new Service
            {
                ServiceNumber = new ServiceNumber("22222")
            };

            var logger = new Mock<ILogger<BackofficeProcessor>>();
            var options = new Mock<IOptions<ServicesFilesOptions>>();
            var parser = new Mock<IParser<TransactionQueue>>();
            var centralServices = new Mock<ICentralServicesList>();
            centralServices.Setup(centralServices => centralServices.Contains(sourceService.ServiceNumber))
                .Returns(true);
            centralServices.Setup(centralServices => centralServices.Get(sourceService.ServiceNumber))
                .Returns(sourceService);
            centralServices.Setup(centralServices => centralServices.Contains(destinationService.ServiceNumber))
                .Returns(true);
            centralServices.Setup(centralServices => centralServices.Get(destinationService.ServiceNumber))
                .Returns(destinationService);

            var processor = new BackofficeProcessor(logger.Object, options.Object, parser.Object, centralServices.Object);
            var record = new TransactionRecord(TransactionCode.CHG)
            {
                SourceNumber = new ServiceNumber("11111"),
                NumberTickets = new NumberTickets(7),
                DestinationNumber = new ServiceNumber("22222"),
                ServiceName = new ServiceName(),
                ServiceDate = new ServiceDate()
            };
        
            // Act
            var result = processor.ProcessCHG(record);
        
            // Assert
            Assert.True(result);
            Assert.Equal(new NumberTickets(1), sourceService.TicketsSold);
            Assert.Equal(new NumberTickets(7), destinationService.TicketsSold);
        }

        [Fact]
        public void ProcessCHG_SourceServiceDoesNotExist_NotProcessed()
        {
            // Arrange
            var sourceService = new Service
            {
                ServiceNumber = new ServiceNumber("11111"),
                TicketsSold = new NumberTickets(8)
            };
            var destinationService = new Service
            {
                ServiceNumber = new ServiceNumber("22222")
            };

            var logger = new Mock<ILogger<BackofficeProcessor>>();
            var options = new Mock<IOptions<ServicesFilesOptions>>();
            var parser = new Mock<IParser<TransactionQueue>>();
            var centralServices = new Mock<ICentralServicesList>();
            centralServices.Setup(centralServices => centralServices.Contains(sourceService.ServiceNumber))
                .Returns(false);
            centralServices.Setup(centralServices => centralServices.Contains(destinationService.ServiceNumber))
                .Returns(true);
            centralServices.Setup(centralServices => centralServices.Get(destinationService.ServiceNumber))
                .Returns(destinationService);

            var processor = new BackofficeProcessor(logger.Object, options.Object, parser.Object, centralServices.Object);
            var record = new TransactionRecord(TransactionCode.CHG)
            {
                SourceNumber = new ServiceNumber("11111"),
                NumberTickets = new NumberTickets(7),
                DestinationNumber = new ServiceNumber("22222"),
                ServiceName = new ServiceName(),
                ServiceDate = new ServiceDate()
            };
        
            // Act
            var result = processor.ProcessCHG(record);
        
            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ProcessCHG_DestinationServiceDoesNotExist_NotProcessed()
        {
            // Arrange
            var sourceService = new Service
            {
                ServiceNumber = new ServiceNumber("11111"),
                TicketsSold = new NumberTickets(8)
            };
            var destinationService = new Service
            {
                ServiceNumber = new ServiceNumber("22222")
            };

            var logger = new Mock<ILogger<BackofficeProcessor>>();
            var options = new Mock<IOptions<ServicesFilesOptions>>();
            var parser = new Mock<IParser<TransactionQueue>>();
            var centralServices = new Mock<ICentralServicesList>();
            centralServices.Setup(centralServices => centralServices.Contains(sourceService.ServiceNumber))
                .Returns(true);
            centralServices.Setup(centralServices => centralServices.Get(sourceService.ServiceNumber))
                .Returns(sourceService);
            centralServices.Setup(centralServices => centralServices.Contains(destinationService.ServiceNumber))
                .Returns(false);

            var processor = new BackofficeProcessor(logger.Object, options.Object, parser.Object, centralServices.Object);
            var record = new TransactionRecord(TransactionCode.CHG)
            {
                SourceNumber = new ServiceNumber("11111"),
                NumberTickets = new NumberTickets(7),
                DestinationNumber = new ServiceNumber("22222"),
                ServiceName = new ServiceName(),
                ServiceDate = new ServiceDate()
            };
        
            // Act
            var result = processor.ProcessCHG(record);
        
            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ProcessCHG_SourceTicketsSoldExceeded_NotProcessed()
        {
            // Arrange
            var sourceService = new Service
            {
                ServiceNumber = new ServiceNumber("11111"),
                TicketsSold = new NumberTickets(1)
            };
            var destinationService = new Service
            {
                ServiceNumber = new ServiceNumber("22222")
            };

            var logger = new Mock<ILogger<BackofficeProcessor>>();
            var options = new Mock<IOptions<ServicesFilesOptions>>();
            var parser = new Mock<IParser<TransactionQueue>>();
            var centralServices = new Mock<ICentralServicesList>();
            centralServices.Setup(centralServices => centralServices.Contains(sourceService.ServiceNumber))
                .Returns(true);
            centralServices.Setup(centralServices => centralServices.Get(sourceService.ServiceNumber))
                .Returns(sourceService);
            centralServices.Setup(centralServices => centralServices.Contains(destinationService.ServiceNumber))
                .Returns(true);
            centralServices.Setup(centralServices => centralServices.Get(destinationService.ServiceNumber))
                .Returns(destinationService);

            var processor = new BackofficeProcessor(logger.Object, options.Object, parser.Object, centralServices.Object);
            var record = new TransactionRecord(TransactionCode.CHG)
            {
                SourceNumber = new ServiceNumber("11111"),
                NumberTickets = new NumberTickets(7),
                DestinationNumber = new ServiceNumber("22222"),
                ServiceName = new ServiceName(),
                ServiceDate = new ServiceDate()
            };
        
            // Act
            var result = processor.ProcessCHG(record);
        
            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ProcessCHG_DestinationCapacityExceeded_NotProcessed()
        {
            // Arrange
            var sourceService = new Service
            {
                ServiceNumber = new ServiceNumber("11111"),
                TicketsSold = new NumberTickets(8)
            };
            var destinationService = new Service
            {
                ServiceNumber = new ServiceNumber("22222"),
                ServiceCapacity = 2
            };

            var logger = new Mock<ILogger<BackofficeProcessor>>();
            var options = new Mock<IOptions<ServicesFilesOptions>>();
            var parser = new Mock<IParser<TransactionQueue>>();
            var centralServices = new Mock<ICentralServicesList>();
            centralServices.Setup(centralServices => centralServices.Contains(sourceService.ServiceNumber))
                .Returns(true);
            centralServices.Setup(centralServices => centralServices.Get(sourceService.ServiceNumber))
                .Returns(sourceService);
            centralServices.Setup(centralServices => centralServices.Contains(destinationService.ServiceNumber))
                .Returns(true);
            centralServices.Setup(centralServices => centralServices.Get(destinationService.ServiceNumber))
                .Returns(destinationService);

            var processor = new BackofficeProcessor(logger.Object, options.Object, parser.Object, centralServices.Object);
            var record = new TransactionRecord(TransactionCode.CHG)
            {
                SourceNumber = new ServiceNumber("11111"),
                NumberTickets = new NumberTickets(7),
                DestinationNumber = new ServiceNumber("22222"),
                ServiceName = new ServiceName(),
                ServiceDate = new ServiceDate()
            };
        
            // Act
            var result = processor.ProcessCHG(record);
        
            // Assert
            Assert.False(result);
        }
    }
}
