using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TelecomProviderAPI.Infrastructure.Repositories;
using TopUpAPI.DataAccess;
using TopUpAPI.Models;
using Xunit;

namespace MobileRecharge.UnitTests.Service
{
    public class MobileRechargeRepositoryTests
    {
        private readonly Mock<TopUpDbContext> _mockContext;
        private readonly Mock<HttpClient> _mockHttpClient;
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<IMapper> _mockMapper;
        private readonly MobileRechargeRepository _repository;

        public MobileRechargeRepositoryTests()
        {
            _mockContext = new Mock<TopUpDbContext>();
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockHttpClient = new Mock<HttpClient>();
            _mockMapper = new Mock<IMapper>();

            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(_mockHttpClient.Object);
            _repository = new MobileRechargeRepository(_mockContext.Object, _mockHttpClientFactory.Object, null, null, _mockMapper.Object);
        }

        [Fact]
        public async Task GetTopUpOptions_ReturnsTopUpOptions()
        {
            // Arrange
            var options = new List<TopUpOption>
        {
            new TopUpOption { Amount = 10 },
            new TopUpOption { Amount = 20 }
        }.AsQueryable();

            var mockSet = new Mock<DbSet<TopUpOption>>();
            mockSet.As<IQueryable<TopUpOption>>().Setup(m => m.Provider).Returns(options.Provider);
            mockSet.As<IQueryable<TopUpOption>>().Setup(m => m.Expression).Returns(options.Expression);
            mockSet.As<IQueryable<TopUpOption>>().Setup(m => m.ElementType).Returns(options.ElementType);
            mockSet.As<IQueryable<TopUpOption>>().Setup(m => m.GetEnumerator()).Returns(options.GetEnumerator());

            _mockContext.Setup(c => c.TopUpOptions).Returns(mockSet.Object);

            // Act
            var result = await _repository.GetTopUpOptions();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task TopUpBeneficiary_ThrowsException_WhenUserNotFound()
        {
            // Arrange
            int userId = 1, beneficiaryId = 1, amount = 10;
            _mockContext.Setup(c => c.Users).Returns(GetTestUsers().Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _repository.TopUpBeneficiary(userId, beneficiaryId, amount));
        }

        [Fact]
        public async Task TopUpBeneficiary_ReturnsTrue_WhenSuccessful()
        {
            // Arrange
            int userId = 1, beneficiaryId = 1;
            decimal amount = 10;
            var user = new User { Id = userId, AvailableBalance = 50 };
            var beneficiary = new Beneficiary { Id = beneficiaryId, MonthlyTopUpLimit = 100 };

            var beneficiaries = new List<Beneficiary> { beneficiary };
            user.Beneficiaries = beneficiaries;

            _mockContext.Setup(c => c.Users).Returns(GetTestUsers(user).Object);
            _mockContext.Setup(c => c.Beneficiaries).Returns(GetTestBeneficiaries().Object);

            // Correctly set up HttpResponseMessage
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK); // Use HttpStatusCode to set the status
            _mockHttpClient.Setup(x => x.PutAsync(It.IsAny<string>(), It.IsAny<StringContent>()))
                .ReturnsAsync(httpResponseMessage);

            // Act
            var result = await _repository.TopUpBeneficiary(userId, beneficiaryId, amount);

            // Assert
            Assert.True(result);
        }

        // Helper methods to create mock DbSet<User> and DbSet<Beneficiary>
        private Mock<DbSet<User>> GetTestUsers(User user = null)
        {
            var users = new List<User>();
            if (user != null)
            {
                users.Add(user);
            }

            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.AsQueryable().Provider);
            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.AsQueryable().Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.AsQueryable().ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.AsQueryable().GetEnumerator());

            return mockSet;
        }

        private Mock<DbSet<Beneficiary>> GetTestBeneficiaries()
        {
            var beneficiaries = new List<Beneficiary>
        {
            new Beneficiary { Id = 1, MonthlyTopUpLimit = 100 }
        }.AsQueryable();

            var mockSet = new Mock<DbSet<Beneficiary>>();
            mockSet.As<IQueryable<Beneficiary>>().Setup(m => m.Provider).Returns(beneficiaries.Provider);
            mockSet.As<IQueryable<Beneficiary>>().Setup(m => m.Expression).Returns(beneficiaries.Expression);
            mockSet.As<IQueryable<Beneficiary>>().Setup(m => m.ElementType).Returns(beneficiaries.ElementType);
            mockSet.As<IQueryable<Beneficiary>>().Setup(m => m.GetEnumerator()).Returns(beneficiaries.GetEnumerator());

            return mockSet;
        }
    }
}
