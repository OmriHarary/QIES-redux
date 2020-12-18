using Xunit;
using QIES.Common;
using QIES.Common.Record;

namespace QIES.Backoffice.Parser.Tests
{
    public class CentralServicesListTests
    {
        [Fact]
        public void Contains_AfterAdd_ReturnedTrue()
        {
            // Arrange
            var serviceNumber = new ServiceNumber("11111");
            var service = new Service
            {
                ServiceNumber = serviceNumber
            };

            var servicesList = new CentralServicesList();

            // Act
            servicesList.Add(service);
            var contained = servicesList.Contains(serviceNumber);

            // Assert
            Assert.True(contained);
        }

        [Fact]
        public void Contains_AfterAddThenDelete_ReturnedFalse()
        {
            // Arrange
            var serviceNumber = new ServiceNumber("12345");
            var service = new Service
            {
                ServiceNumber = serviceNumber
            };

            var servicesList = new CentralServicesList();

            // Act
            servicesList.Add(service);
            servicesList.Delete(serviceNumber);
            var contained = servicesList.Contains(serviceNumber);

            // Assert
            Assert.False(contained);
        }

        [Fact]
        public void Get_AfterAdd_SameServiceReturned()
        {
            // Arrange
            var serviceNumber = new ServiceNumber("22222");
            var service = new Service
            {
                ServiceNumber = serviceNumber
            };

            var servicesList = new CentralServicesList();

            // Act
            servicesList.Add(service);
            var serviceGot = servicesList.Get(serviceNumber);

            // Assert
            Assert.Equal(service, serviceGot);
        }

        [Fact]
        public void ValidServicesFileContentsRequirements()
        {
            // Arrange
            var serviceNumber1 = new ServiceNumber("99999");
            var service1 = new Service
            {
                ServiceNumber = serviceNumber1
            };

            var serviceNumber2 = new ServiceNumber("11111");
            var service2 = new Service
            {
                ServiceNumber = serviceNumber2
            };

            var servicesList = new CentralServicesList();

            // Act
            servicesList.Add(service1);
            servicesList.Add(service2);
            var result = servicesList.ValidServicesFileContents();

            // Assert
            string[] expected = { "11111", "99999", "00000" };

            Assert.Equal(expected, result);
        }

        [Fact]
        public void CentralServicesFileContentsRequirements()
        {
            // Arrange
            var serviceNumber1 = new ServiceNumber("99999");
            var service1 = new Service
            {
                ServiceNumber = serviceNumber1,
                ServiceName = new ServiceName("service1"),
                ServiceCapacity = 100
            };

            var serviceNumber2 = new ServiceNumber("11111");
            var service2 = new Service
            {
                ServiceNumber = serviceNumber2,
                ServiceName = new ServiceName("service2"),
                ServiceCapacity = 200
            };

            var servicesList = new CentralServicesList();

            // Act
            servicesList.Add(service1);
            servicesList.Add(service2);
            var result = servicesList.CentralServicesFileContents();

            // Assert
            string[] expected = { "11111 200 0 service2", "99999 100 0 service1" };

            Assert.Equal(expected, result);
        }
    }
}
