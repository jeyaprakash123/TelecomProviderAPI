using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore;
using TelecomProviderAPI.Infrastructure.Repositories;
using TopUpAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using TopUpAPI.DataAccess;
using Microsoft.Extensions.Configuration;

namespace TelecomProviderAPI.Tests
{
    public class UserRepositoryTests
    {
        private readonly Mock<TopUpDbContext> _mockContext;
        private readonly Mock<DbSet<User>> _mockUserSet;
        private readonly UserRepository _userRepository;

        public UserRepositoryTests()
        {
            _mockContext = new Mock<TopUpDbContext>();
            _mockUserSet = new Mock<DbSet<User>>();

            _userRepository = new UserRepository(_mockContext.Object, new Mock<IConfiguration>().Object,
                                                 new Mock<IMapper>().Object, new Mock<IHttpClientFactory>().Object);
        }

        [Fact]
        public async Task GetUsersAsync_ReturnsAllUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = 1, Username = "User 1" },
                new User { Id = 2, Username = "User 2" }
            }.AsQueryable();

            _mockUserSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            _mockUserSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            _mockUserSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            _mockUserSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            _mockContext.Setup(c => c.Users).Returns(_mockUserSet.Object);

            // Act
            var result = await _userRepository.GetUsersAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetUser_ReturnsUser_WhenUserExists()
        {
            // Arrange
            var user = new User { Id = 1, Username = "User 1" };

            _mockUserSet.Setup(m => m.FindAsync(It.IsAny<object[]>())).ReturnsAsync(user);
            _mockContext.Setup(c => c.Users).Returns(_mockUserSet.Object);

            // Act
            var result = await _userRepository.GetUser(1);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal("User 1", result.Value.Username); // Access the Username from the Value property
        }


        [Fact]
        public async Task CreateUserAsync_AddsUser()
        {
            // Arrange
            var user = new User { Id = 1, Username = "User 1" };

            _mockContext.Setup(c => c.Users).Returns(_mockUserSet.Object);
            _mockUserSet.Setup(m => m.Add(user)).Callback<User>(u =>
            {
                var usersList = new List<User>();
                usersList.Add(u);
                _mockUserSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(usersList.AsQueryable().Provider);
            });

            // Act
            var result = await _userRepository.CreateUserAsync(user);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("User 1", result.Username);
            _mockUserSet.Verify(m => m.Add(user), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_UpdatesUser_WhenUserExists()
        {
            // Arrange
            var user = new User { Id = 1, Username = "User 1", IsVerified = false };

            _mockUserSet.Setup(m => m.FindAsync(It.IsAny<object[]>())).ReturnsAsync(user);
            _mockContext.Setup(c => c.Users).Returns(_mockUserSet.Object);

            // Act
            var result = await _userRepository.UpdateUserAsync(1, true);

            // Assert
            Assert.True(result);
            Assert.True(user.IsVerified);
        }

        [Fact]
        public async Task DeleteUserAsync_DeletesUser_WhenUserExists()
        {
            // Arrange
            var user = new User { Id = 1, Username = "User 1" };

            _mockUserSet.Setup(m => m.FindAsync(It.IsAny<object[]>())).ReturnsAsync(user);
            _mockContext.Setup(c => c.Users).Returns(_mockUserSet.Object);

            // Act
            var result = await _userRepository.DeleteUserAsync(1);

            // Assert
            Assert.True(result);
            _mockUserSet.Verify(m => m.Remove(user), Times.Once);
        }
    }
}
