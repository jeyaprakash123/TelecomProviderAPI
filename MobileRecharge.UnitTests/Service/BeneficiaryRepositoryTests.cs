using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MobileRecharge.Domain.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelecomProviderAPI.Core.IRepository;
using TelecomProviderAPI.Infrastructure.Repositories;
using TopUpAPI.DataAccess;
using TopUpAPI.Models;
using Xunit;
namespace MobileRecharge.UnitTests.Service
{
    public class BeneficiaryRepositoryTests
    {
        private readonly Mock<TopUpDbContext> _mockContext;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IOptions<Appsettings>> _mockAppSettings;
        private readonly BeneficiaryRepository _repository;
        private readonly Appsettings _appSettings;

        public BeneficiaryRepositoryTests()
        {
            _mockContext = new Mock<TopUpDbContext>();
            _mockConfig = new Mock<IConfiguration>();
            _mockMapper = new Mock<IMapper>();
            _mockAppSettings = new Mock<IOptions<Appsettings>>();

            _appSettings = new Appsettings
            {
                MaxBeneficiaryPerUser = 5,
                MaximumRechargePerMonthForVerifiedUser = 1000,
                MaximumRechargePerMonthForNotverifiedUser = 500
            };

            _mockAppSettings.Setup(a => a.Value).Returns(_appSettings);
            _repository = new BeneficiaryRepository(_mockContext.Object, _mockConfig.Object, _mockMapper.Object, _mockAppSettings.Object);
        }

        [Fact]
        public async Task GetBeneficiaries_ReturnsListOfBeneficiaries()
        {
            // Arrange
            int userId = 1;
            var beneficiaries = new List<Beneficiary>
            {
                new Beneficiary { Id = 1, UserId = userId, Nickname = "Beneficiary1" },
                new Beneficiary { Id = 2, UserId = userId, Nickname = "Beneficiary2" }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Beneficiary>>();
            mockSet.As<IQueryable<Beneficiary>>().Setup(m => m.Provider).Returns(beneficiaries.Provider);
            mockSet.As<IQueryable<Beneficiary>>().Setup(m => m.Expression).Returns(beneficiaries.Expression);
            mockSet.As<IQueryable<Beneficiary>>().Setup(m => m.ElementType).Returns(beneficiaries.ElementType);
            mockSet.As<IQueryable<Beneficiary>>().Setup(m => m.GetEnumerator()).Returns(beneficiaries.GetEnumerator());

            _mockContext.Setup(c => c.Beneficiaries).Returns(mockSet.Object);

            // Act
            var result = await _repository.GetBeneficiaries(userId);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task AddBeneficiary_Succeeds_WhenValid()
        {
            // Arrange
            int userId = 1;
            string nickname = "NewBeneficiary";
            var user = new User { Id = userId, IsVerified = true, Beneficiaries = new List<Beneficiary>() };

            _mockContext.Setup(c => c.Users).Returns(GetTestUsers(user).Object);
            _mockContext.Setup(c => c.Beneficiaries).Returns(GetTestBeneficiaries().Object);

            // Act
            var result = await _repository.AddBeneficiary(userId, nickname);

            // Assert
            Assert.True(result);
            _mockContext.Verify(c => c.Beneficiaries.Add(It.IsAny<Beneficiary>()), Times.Once);
        }

        [Fact]
        public async Task AddBeneficiary_ThrowsException_WhenNicknameIsInvalid()
        {
            // Arrange
            int userId = 1;
            string nickname = ""; // Invalid nickname
            var user = new User { Id = userId, IsVerified = true, Beneficiaries = new List<Beneficiary>() };

            _mockContext.Setup(c => c.Users).Returns(GetTestUsers(user).Object);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _repository.AddBeneficiary(userId, nickname));
            Assert.Equal("Nickname must be 20 characters or less.", ex.Message);
        }

        [Fact]
        public async Task AddBeneficiary_ThrowsException_WhenUserNotFound()
        {
            // Arrange
            int userId = 1;
            string nickname = "NewBeneficiary";

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _repository.AddBeneficiary(userId, nickname));
            Assert.Equal("User not found.", ex.Message);
        }

        [Fact]
        public async Task DeleteBeneficiary_ReturnsTrue_WhenSuccessful()
        {
            // Arrange
            int beneficiaryId = 1;
            var beneficiary = new Beneficiary { Id = beneficiaryId };

            _mockContext.Setup(c => c.Beneficiaries.FindAsync(beneficiaryId)).ReturnsAsync(beneficiary);

            // Act
            var result = await _repository.DeleteBeneficiary(beneficiaryId);

            // Assert
            Assert.True(result);
            _mockContext.Verify(c => c.Beneficiaries.Remove(beneficiary), Times.Once);
        }

        [Fact]
        public async Task DeleteBeneficiary_ReturnsFalse_WhenNotFound()
        {
            // Arrange
            int beneficiaryId = 1;

            _mockContext.Setup(c => c.Beneficiaries.FindAsync(beneficiaryId)).ReturnsAsync((Beneficiary)null);

            // Act
            var result = await _repository.DeleteBeneficiary(beneficiaryId);

            // Assert
            Assert.False(result);
        }

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
            var beneficiaries = new List<Beneficiary>().AsQueryable();

            var mockSet = new Mock<DbSet<Beneficiary>>();
            mockSet.As<IQueryable<Beneficiary>>().Setup(m => m.Provider).Returns(beneficiaries.Provider);
            mockSet.As<IQueryable<Beneficiary>>().Setup(m => m.Expression).Returns(beneficiaries.Expression);
            mockSet.As<IQueryable<Beneficiary>>().Setup(m => m.ElementType).Returns(beneficiaries.ElementType);
            mockSet.As<IQueryable<Beneficiary>>().Setup(m => m.GetEnumerator()).Returns(beneficiaries.GetEnumerator());

            return mockSet;
        }
    }
}