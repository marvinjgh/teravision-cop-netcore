using Backend.API.Controllers;
using Backend.Service.Contracts;
using Backend.Service.DataTransferObjects;
using Backend.Service.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Backend.Tests;
#pragma warning disable CS8600, CS8602
public class ProjectControllerTests
{
    [Fact]
    public async Task GetProjectById_ReturnsNotFound()
    {
        // Arrange
        long testId = 1;

        var mockRepo = new Mock<IRepositoryWrapper>();
        mockRepo.Setup(repo => repo.Project.GetProjectById(testId))
            .ReturnsAsync((Project?)null);

        var controller = new ProjectController(mockRepo.Object);

        //Act
        var result = await controller.GetProject(testId);

        //Assert
        var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
        ErrorDTO error = (ErrorDTO)notFoundObjectResult.Value;
        Assert.Equal("Project not found", error.Message);

        mockRepo.Verify(
            repo => repo.Project.GetProjectById(testId),
            Times.Once
        );
    }

    [Fact]
    public async Task GetProjectById_ReturnsOk()
    {
        // Arrange
        long testId = 1;

        var mockRepo = new Mock<IRepositoryWrapper>();
        mockRepo.Setup(repo => repo.Project.GetProjectById(testId))
            .ReturnsAsync(new Project
            {
                Id = 1,
                Name = "Project 1"
            });

        var controller = new ProjectController(mockRepo.Object);

        //Act
        var result = await controller.GetProject(testId);

        //Assert

        var okObjectResult = Assert.IsType<OkObjectResult>(result);

        var dtoTest = new Project
        {
            Id = 1,
            Name = "Project 1"
        };
        var dtoResult = (Project)okObjectResult.Value;

        Assert.Equal(dtoTest.Id, dtoResult.Id);
        Assert.Equal(dtoTest.Name, dtoResult.Name);

        mockRepo.Verify(
            repo => repo.Project.GetProjectById(testId),
            Times.Once
        );
    }

    [Fact]
    public async Task GetAllProjects_ReturnsOk()
    {
        // Arrange
        var mockRepo = new Mock<IRepositoryWrapper>();
        mockRepo.Setup(repo => repo.Project.GetAllProjects())
            .ReturnsAsync(new List<Project>
            {
                new Project { Id = 1, Name = "Project 1" },
                new Project { Id = 2, Name = "Project 2" }
            });

        var controller = new ProjectController(mockRepo.Object);

        //Act
        var result = await controller.GetAllProjects();

        //Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(result);
        var projects = Assert.IsAssignableFrom<List<Project>>(okObjectResult.Value);
        Assert.Equal(2, projects.Count);

        mockRepo.Verify(
            repo => repo.Project.GetAllProjects(),
            Times.Once
        );
    }

    [Fact]
    public async Task PostProject_ReturnsBadRequest()
    {
        // Arrange: create mocks and simulate new project Id assignment
        var mockRepository = new Mock<IRepositoryWrapper>();

        var controller = new ProjectController(mockRepository.Object);
        var projectCreateDTO = new ProjectCreateDTO();

        controller.ModelState.AddModelError("Name", "The Name field is required.");

        // Act
        var result = await controller.PostProject(projectCreateDTO);

        // Assert
        var createdResult = Assert.IsType<BadRequestObjectResult>(result);
        ErrorDTO error = (ErrorDTO)createdResult.Value;
        Assert.Equal("Invalid model object", error.Message);

        mockRepository.Verify(
            repo => repo.Project.CreateProject(It.IsAny<Project>()),
            Times.Never
        );
        mockRepository.Verify(
            repo => repo.Save(),
            Times.Never
        );
    }

    [Fact]
    public async Task PostProject_ReturnsCreatedAtAction()
    {
        // Arrange: create mocks and simulate new project Id assignment
        var now = DateTimeOffset.UtcNow;
        var projectCreateDTO = new ProjectCreateDTO
        {
            Name = "Test Project",
            Description = "Test Description"
        };

        var mockRepository = new Mock<IRepositoryWrapper>();
        var mockProjectRepo = new Mock<IProjectRepository>();
        mockProjectRepo.Setup(r => r.CreateProject(It.IsAny<Project>()))
            .Callback<Project>(p =>
            {
                p.Id = 1;
                p.Name = projectCreateDTO.Name;
                p.Description = projectCreateDTO.Description;
                p.CreatedAt = now;
            });
        mockRepository.Setup(r => r.Project).Returns(mockProjectRepo.Object);
        mockRepository.Setup(r => r.Save()).Returns(Task.CompletedTask);

        var controller = new ProjectController(mockRepository.Object);

        // Act
        var result = await controller.PostProject(projectCreateDTO);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.NotNull(createdResult.RouteValues);
        Assert.Equal((long)1, createdResult.RouteValues["id"]);

        var resultProject = (Project)createdResult.Value;

        Assert.Equal(1, resultProject.Id);
        Assert.Equal(projectCreateDTO.Name, resultProject.Name);
        Assert.Equal(projectCreateDTO.Description, resultProject.Description);
        Assert.Equal(now, resultProject.CreatedAt);

        // Opcional: Verificar que el método Save nunca fue llamado.
        mockRepository.Verify(
            repo => repo.Project.CreateProject(It.IsAny<Project>()),
            Times.Once
        );
        mockRepository.Verify(
            repo => repo.Save(),
            Times.Once
        );
    }
}
#pragma warning restore CS8600, CS8602