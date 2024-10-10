using Moq;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using TelecomProviderAPI.Application.Interfaces;
using TelecomProviderAPI.Core.IRepository;
using TopUpAPI.Models;
using MobileRecharge.Domain.UnitOfWork;
using AutoMapper;
using TopUpAPI.Services;
using TopUpAPI.DataMapper;

namespace MobileRecharge.UnitTests.Service
{
    public class UserServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _service = new UserService(_unitOfWorkMock.Object, null, _mapperMock.Object, null);
        }

        [Fact]
        public async Task GetUsersAsync_ReturnsMappedUserDtos()
        {
            // Arrange
            var users = new List<User> { new User { Id = 1, Username = "TestUser" } };
            var userDtos = new List<UserDto> { new UserDto { Id = 1, Username = "TestUser" } };
            _unitOfWorkMock.Setup(repo => repo.UserRepository.GetUsersAsync()).ReturnsAsync(users);
            _mapperMock.Setup(m => m.Map<IEnumerable<UserDto>>(users)).Returns(userDtos);

            // Act
            var result = await _service.GetUsersAsync();

            // Assert
            Assert.Equal(1, result.Count());
            Assert.Equal("TestUser", result.First().Username);
        }

        [Fact]
        public async Task CreateUserAsync_ValidUser_CreatesUser()
        {
            // Arrange
            var user = new User { Id = 1, Username = "NewUser" };
            _unitOfWorkMock.Setup(repo => repo.UserRepository.CreateUserAsync(user)).ReturnsAsync(user);
            _unitOfWorkMock.Setup(repo => repo.CompleteAsync()).ReturnsAsync(1); // Simulate successful save

            // Act
            var result = await _service.CreateUserAsync(user);

            // Assert
            Assert.Equal(user.Id, result.Id);
        }

        // Additional tests for UpdateUserAsync and DeleteUserAsync can be added here...
    }
}

