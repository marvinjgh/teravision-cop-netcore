using System.Linq.Expressions;
using Backend.API.Controllers;
using Backend.Service.Contracts;
using Backend.Service.DataTransferObjects;
using Backend.Service.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Backend.Tests;

public class AuthControllerTests
{
#pragma warning disable CS8600, CS8602, CS8620, CS8625
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockRepo = new Mock<IRepositoryWrapper>();
        _mockUserRepo = new Mock<IUserRepository>();
        _mockConfig = new Mock<IConfiguration>();

        // Setup nested repository
        _mockRepo.Setup(r => r.UserRepository).Returns(_mockUserRepo.Object);

        // Fake config values for JWT
        _mockConfig.Setup(c => c["AppSettings:Token"]).Returns("SuperSecretTestKeySuperSecretTestKeySuperSecretTestKeySuperSecretTestKey");
        _mockConfig.Setup(c => c["AppSettings:Issuer"]).Returns("TestIssuer");
        _mockConfig.Setup(c => c["AppSettings:Audience"]).Returns("TestAudience");

        _controller = new AuthController(_mockRepo.Object, _mockConfig.Object);
    }

    // ---------------- REGISTER ----------------

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenRequestIsNull()
    {
        // Act
        var result = await _controller.Register(null);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var error = Assert.IsType<ErrorDTO>(badRequest.Value);
        Assert.Equal("Empty request", error.Message);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenUsernameExists()
    {
        // Arrage
        var request = new UserDTO { Username = "test", Password = "password", Name = "name", Email = "email@host.com" };
        var existingUser = new UserEntity { Username = "test" };

        _mockUserRepo.Setup(r => r.GetAllUsers(It.IsAny<Expression<Func<UserEntity, bool>>>()))
                     .ReturnsAsync([existingUser]);

        // Act
        var result = await _controller.Register(request);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var error = Assert.IsType<ErrorDTO>(badRequest.Value);
        Assert.Equal("Username already exist.", error.Message);
        _mockUserRepo.Verify(r => r.GetAllUsers(It.IsAny<Expression<Func<UserEntity, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task Register_ReturnsOk_WhenUserCreated()
    {
        // Arrange
        var request = new UserDTO { Username = "newuser", Password = "password", Name = "name", Email = "email@host.com" };

        _mockUserRepo.Setup(r => r.GetAllUsers(It.IsAny<Expression<Func<UserEntity, bool>>>()))
                     .ReturnsAsync([]);

        _mockUserRepo.Setup(r => r.CreateUser(It.IsAny<UserEntity>()));
        _mockRepo.Setup(r => r.Save()).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Register(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var user = Assert.IsType<UserEntity>(okResult.Value);
        Assert.Equal("newuser", user.Username);
        Assert.NotNull(user.PasswordHash);
        _mockRepo.Verify(
            repo => repo.UserRepository.CreateUser(It.IsAny<UserEntity>()),
            Times.Once
        );
        _mockRepo.Verify(repo => repo.Save(), Times.Once);
    }


    // ---------------- LOGIN ----------------

    [Fact]
    public async Task Login_ReturnsBadRequest_WhenRequestIsNull()
    {
        // Act
        var result = await _controller.Login(null);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var error = Assert.IsType<ErrorDTO>(badRequest.Value);
        Assert.Equal("Empty request", error.Message);
    }

    [Fact]
    public async Task Login_ReturnsBadRequest_WhenUserNotFound()
    {
        // Arrage
        var request = new LoginDTO { Username = "nouser", Password = "pw" };
        _mockUserRepo.Setup(r => r.GetAllUsers(It.IsAny<Expression<Func<UserEntity, bool>>>()))
                     .ReturnsAsync([]);

        // Act
        var result = await _controller.Login(request);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var error = Assert.IsType<ErrorDTO>(badRequest.Value);
        Assert.Equal("Invalid Username or Password.", error.Message);
        _mockRepo.Verify(
            repo => repo.UserRepository.GetAllUsers(It.IsAny<Expression<Func<UserEntity, bool>>>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Login_ReturnsBadRequest_WhenPasswordIsInvalid()
    {
        // Arrage
        var request = new LoginDTO { Username = "test", Password = "wrongpw" };
        var user = new UserEntity
        {
            Id = 1,
            Username = "test",
            PasswordHash = new PasswordHasher<UserEntity>().HashPassword(null, "correctpw"),
            Role = "User"
        };

        _mockUserRepo.Setup(r => r.GetAllUsers(It.IsAny<Expression<Func<UserEntity, bool>>>()))
                     .ReturnsAsync([user]);

        // Act
        var result = await _controller.Login(request);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var error = Assert.IsType<ErrorDTO>(badRequest.Value);
        Assert.Equal("Invalid Username or Password.", error.Message);
        _mockRepo.Verify(
            repo => repo.UserRepository.GetAllUsers(It.IsAny<Expression<Func<UserEntity, bool>>>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Login_ReturnsOk_WhenCredentialsAreValid()
    {
        // Arrage
        var request = new LoginDTO { Username = "valid", Password = "password123" };
        var user = new UserEntity
        {
            Id = 1,
            Username = "valid",
            PasswordHash = new PasswordHasher<UserEntity>().HashPassword(null, "password123"),
            Role = "User"
        };

        _mockUserRepo.Setup(r => r.GetAllUsers(It.IsAny<Expression<Func<UserEntity, bool>>>()))
                     .ReturnsAsync([user]);
        _mockRepo.Setup(r => r.Save()).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Login(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var token = Assert.IsType<TokenResponseDTO>(okResult.Value);
        Assert.NotNull(token.AccessToken);
        Assert.NotNull(token.RefreshToken);
        _mockRepo.Verify(
            repo => repo.UserRepository.GetAllUsers(It.IsAny<Expression<Func<UserEntity, bool>>>()),
            Times.Once
        );
        _mockRepo.Verify(r => r.Save(), Times.Once);
        _mockConfig.Verify(repo => repo["AppSettings:Token"], Times.Once);
        _mockConfig.Verify(repo => repo["AppSettings:Issuer"], Times.Once);
        _mockConfig.Verify(repo => repo["AppSettings:Audience"], Times.Once);
    }

    // ---------------- REFRESH TOKEN ----------------

    [Fact]
    public async Task RefreshToken_ReturnsBadRequest_WhenRequestIsNull()
    {
        // Act
        var result = await _controller.RefreshToken(null);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var error = Assert.IsType<ErrorDTO>(badRequest.Value);
        Assert.Equal("Empty request", error.Message);
    }

    [Fact]
    public async Task RefreshToken_ReturnsBadRequest_WhenUserIsNull()
    {
        // Arrange
        var request = new RefreshTokenDTO { UserId = 1, RefreshToken = "invalid" };

        _mockUserRepo.Setup(r => r.GetUserById(request.UserId)).ReturnsAsync((UserEntity)null);

        // Act
        var result = await _controller.RefreshToken(request);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var error = Assert.IsType<ErrorDTO>(badRequest.Value);
        Assert.Equal("Invalid data.", error.Message);

        _mockRepo.Verify(
            repo => repo.UserRepository.GetUserById(It.IsAny<long>()),
            Times.Once
        );
    }

    [Fact]
    public async Task RefreshToken_ReturnsBadRequest_WhenExpired()
    {
        // Arrange
        var user = new UserEntity
        {
            Id = 1,
            Username = "test",
            Role = "User",
            RefreshToken = "oldtoken",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(-1)
        };

        var request = new RefreshTokenDTO { UserId = user.Id, RefreshToken = "oldtoken" };

        _mockUserRepo.Setup(r => r.GetUserById(user.Id)).ReturnsAsync(user);

        // Act
        var result = await _controller.RefreshToken(request);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var error = Assert.IsType<ErrorDTO>(badRequest.Value);
        Assert.Equal("Invalid data.", error.Message);
        _mockRepo.Verify(
            repo => repo.UserRepository.GetUserById(It.IsAny<long>()),
            Times.Once
        );
    }

    [Fact]
    public async Task RefreshToken_ReturnsOk_WhenValid()
    {
        // Arrage
        var user = new UserEntity
        {
            Id = 1,
            Username = "test",
            Role = "User",
            RefreshToken = "validtoken",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1)
        };
        var request = new RefreshTokenDTO { UserId = user.Id, RefreshToken = "validtoken" };

        _mockUserRepo.Setup(r => r.GetUserById(user.Id)).ReturnsAsync(user);
        _mockRepo.Setup(r => r.Save()).Returns(Task.CompletedTask);

        var result = await _controller.RefreshToken(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var token = Assert.IsType<TokenResponseDTO>(okResult.Value);
        Assert.NotNull(token.AccessToken);
        Assert.NotNull(token.RefreshToken);
        _mockRepo.Verify(
            repo => repo.UserRepository.GetUserById(It.IsAny<long>()),
            Times.Once
        );
        _mockConfig.Verify(repo => repo["AppSettings:Token"], Times.Once);
        _mockConfig.Verify(repo => repo["AppSettings:Issuer"], Times.Once);
        _mockConfig.Verify(repo => repo["AppSettings:Audience"], Times.Once);
    }
#pragma warning restore CS8600, CS8602, CS8620, CS8625
}