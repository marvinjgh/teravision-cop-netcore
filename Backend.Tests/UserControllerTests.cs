using Backend.API.Controllers;
using Backend.Service.Contracts;
using Backend.Service.DataTransferObjects;
using Backend.Service.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Backend.Tests;

public class UserControllerTests
{
    private readonly Mock<IRepositoryWrapper> mockRepository;
    private readonly Mock<IUserRepository> mockUserRepo;
    private readonly UserController controller;

    public UserControllerTests()
    {
        mockRepository = new Mock<IRepositoryWrapper>();
        mockUserRepo = new Mock<IUserRepository>();
        mockRepository.Setup(r => r.UserRepository).Returns(mockUserRepo.Object);
        controller = new UserController(mockRepository.Object);
    }

    [Fact]
    public async Task SearchUser_ReturnsOk_WhenUserExists()
    {
        var user = new UserEntity { Id = 1, Username = "testuser", Name = "Test", Email = "test@test.com" };
        mockUserRepo.Setup(r => r.FindByUsername("testuser")).ReturnsAsync(user);

        var result = await controller.SearchUser("testuser");

        var okResult = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<UserDTO>(okResult.Value);
        Assert.Equal("testuser", dto.Username);
    }

    [Fact]
    public async Task SearchUser_ReturnsNotFound_WhenUserDoesNotExist()
    {
        mockUserRepo.Setup(r => r.FindByUsername("nouser")).ReturnsAsync((UserEntity?)null);

        var result = await controller.SearchUser("nouser");

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var error = Assert.IsType<ErrorDTO>(notFound.Value);
        Assert.Equal("User not found", error.Message);
    }

    [Fact]
    public async Task SearchUser_ReturnsBadRequest_WhenUsernameIsEmpty()
    {
        var result = await controller.SearchUser("");

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var error = Assert.IsType<ErrorDTO>(badRequest.Value);
        Assert.Equal("Username is required", error.Message);
    }

    [Fact]
    public async Task UpdateUser_ReturnsOk_WhenUserIsUpdated()
    {
        var user = new UserEntity { Id = 1, Username = "testuser", Name = "Test", Email = "test@test.com" };
        mockUserRepo.Setup(r => r.GetUserById(1)).ReturnsAsync(user);

        var updatedUser = new UserEntity { Id = 1, Username = "testuser", Name = "Updated", Email = "updated@test.com" };

        var result = await controller.UpdateUser(1, updatedUser);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<UserDTO>(okResult.Value);
        Assert.Equal("Updated", dto.Name);
        Assert.Equal("updated@test.com", dto.Email);
    }

    [Fact]
    public async Task UpdateUser_ReturnsNotFound_WhenUserDoesNotExist()
    {
        mockUserRepo.Setup(r => r.GetUserById(1)).ReturnsAsync((UserEntity?)null);

        var updatedUser = new UserEntity { Id = 1, Username = "testuser", Name = "Updated", Email = "updated@test.com" };

        var result = await controller.UpdateUser(1, updatedUser);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var error = Assert.IsType<ErrorDTO>(notFound.Value);
        Assert.Equal("User not found", error.Message);
    }

    [Fact]
    public async Task UpdateUser_ReturnsBadRequest_WhenUserIsNull()
    {
        var result = await controller.UpdateUser(1, null);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var error = Assert.IsType<ErrorDTO>(badRequest.Value);
        Assert.Equal("Invalid user data", error.Message);
    }

    [Fact]
    public async Task UpdateUser_ReturnsBadRequest_WhenIdMismatch()
    {
        var updatedUser = new UserEntity { Id = 2, Username = "testuser", Name = "Updated", Email = "updated@test.com" };

        var result = await controller.UpdateUser(1, updatedUser);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var error = Assert.IsType<ErrorDTO>(badRequest.Value);
        Assert.Equal("Invalid user data", error.Message);
    }

    [Fact]
    public async Task DeleteUser_ReturnsNoContent_WhenUserDeleted()
    {
        var user = new UserEntity { Id = 1, Username = "testuser" };
        mockUserRepo.Setup(r => r.GetUserById(1)).ReturnsAsync(user);

        var result = await controller.DeleteUser(1);

        Assert.IsType<NoContentResult>(result);
        mockUserRepo.Verify(r => r.DeleteUser(user), Times.Once);
        mockRepository.Verify(r => r.Save(), Times.Once);
    }

    [Fact]
    public async Task DeleteUser_ReturnsNotFound_WhenUserDoesNotExist()
    {
        mockUserRepo.Setup(r => r.GetUserById(1)).ReturnsAsync((UserEntity?)null);

        var result = await controller.DeleteUser(1);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var error = Assert.IsType<ErrorDTO>(notFound.Value);
        Assert.Equal("User not found", error.Message);
    }
}