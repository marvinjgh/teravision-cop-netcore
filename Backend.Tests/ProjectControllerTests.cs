using Backend.API.Controllers;
using Backend.Service.Contracts;
using Backend.Service.DataTransferObjects;
using Backend.Service.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Backend.Tests;
#pragma warning disable CS8600, CS8602, CS8625
public class ProjectControllerTests
{
    [Fact]
    public async Task GetProjectById_ReturnsNotFound()
    {
        // Arrange
        long testId = 1;

        var mockRepo = new Mock<IRepositoryWrapper>();
        mockRepo.Setup(repo => repo.ProjectRepository.GetProjectById(testId))
            .ReturnsAsync((Project?)null);

        var controller = new ProjectController(mockRepo.Object);

        //Act
        var result = await controller.GetProject(testId);

        //Assert
        var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
        ErrorDTO error = (ErrorDTO)notFoundObjectResult.Value;
        Assert.Equal("Project not found", error.Message);

        mockRepo.Verify(
            repo => repo.ProjectRepository.GetProjectById(testId),
            Times.Once
        );
    }

    [Fact]
    public async Task GetProjectById_ReturnsOk()
    {
        // Arrange
        long testId = 1;
        var now = DateTimeOffset.UtcNow;
        var mockRepo = new Mock<IRepositoryWrapper>();
        var projectEntity = new Project
        {
            Id = 1,
            Name = "Project 1",
            CreatedAt = now,
            UpdatedAt = now
        };
        mockRepo.Setup(repo => repo.ProjectRepository.GetProjectById(testId))
            .ReturnsAsync(projectEntity);

        var controller = new ProjectController(mockRepo.Object);

        //Act
        var result = await controller.GetProject(testId);

        //Assert

        var okResult = Assert.IsType<OkObjectResult>(result);
        var projectResult = (Project)okResult.Value;

        Assert.Equal(projectEntity.Id, projectResult.Id);
        Assert.Equal(projectEntity.Name, projectResult.Name);
        Assert.Equal(projectEntity.CreatedAt, projectResult.CreatedAt);
        Assert.Equal(projectEntity.UpdatedAt, projectResult.UpdatedAt);

        mockRepo.Verify(
            repo => repo.ProjectRepository.GetProjectById(testId),
            Times.Once
        );
    }

    [Fact]
    public async Task GetAllProjects_ReturnsOk()
    {
        // Arrange
        var mockRepo = new Mock<IRepositoryWrapper>();
        mockRepo.Setup(repo => repo.ProjectRepository.GetAllProjects())
            .ReturnsAsync(new List<Project>
            {
                new() { Id = 1, Name = "Project 1" },
                new() { Id = 2, Name = "Project 2" }
            });

        var controller = new ProjectController(mockRepo.Object);

        //Act
        var result = await controller.GetAllProjects();

        //Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(result);
        var projects = Assert.IsAssignableFrom<List<Project>>(okObjectResult.Value);
        Assert.Equal(2, projects.Count);

        mockRepo.Verify(
            repo => repo.ProjectRepository.GetAllProjects(),
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
            repo => repo.ProjectRepository.CreateProject(It.IsAny<Project>()),
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
        long testId = 1;
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
                p.UpdatedAt = now;
            });
        mockRepository.Setup(r => r.ProjectRepository).Returns(mockProjectRepo.Object);
        mockRepository.Setup(r => r.Save()).Returns(Task.CompletedTask);

        var controller = new ProjectController(mockRepository.Object);

        // Act
        var result = await controller.PostProject(projectCreateDTO);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.NotNull(createdResult.RouteValues);
        Assert.Equal(testId, createdResult.RouteValues["id"]);

        var resultProject = (Project)createdResult.Value;

        Assert.Equal(testId, resultProject.Id);
        Assert.Equal(projectCreateDTO.Name, resultProject.Name);
        Assert.Equal(projectCreateDTO.Description, resultProject.Description);
        Assert.Equal(now, resultProject.CreatedAt);
        Assert.Equal(now, resultProject.UpdatedAt);

        // Opcional: Verificar que el método Save nunca fue llamado.
        mockRepository.Verify(
            repo => repo.ProjectRepository.CreateProject(It.IsAny<Project>()),
            Times.Once
        );
        mockRepository.Verify(
            repo => repo.Save(),
            Times.Once
        );
    }

    [Fact]
    public async Task DeleteProject_ReturnsOk()
    {
        //Arrange
        long testId = 1;
        Project testProject = new() { Id = testId };

        var mockRepo = new Mock<IRepositoryWrapper>();
        mockRepo.Setup(repo => repo.ProjectRepository.GetProjectById(testId))
            .ReturnsAsync(testProject);

        var controller = new ProjectController(mockRepo.Object);

        //Act
        var result = await controller.DeleteProject(testId);

        //Assert
        var okObjectResult = Assert.IsType<NoContentResult>(result);
        Assert.NotNull(okObjectResult);
        mockRepo.Verify(
            repo => repo.ProjectRepository.DeleteProject(testProject),
            Times.Once
        );
        mockRepo.Verify(
            repo => repo.Save(),
            Times.Once
        );
    }

    [Fact]
    public async Task DeleteProject_ReturnsNotFound()
    {
        // Arrange
        long testId = 1;
        Project testProject = new() { Id = testId };

        var mockRepo = new Mock<IRepositoryWrapper>();
        mockRepo.Setup(repo => repo.ProjectRepository.GetProjectById(testId))
            .ReturnsAsync((Project?)null);

        var controller = new ProjectController(mockRepo.Object);

        //Act
        var result = await controller.DeleteProject(testId);

        //Assert
        var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
        ErrorDTO error = (ErrorDTO)notFoundObjectResult.Value;
        Assert.Equal("Project not found", error.Message);

        mockRepo.Verify(
            repo => repo.ProjectRepository.DeleteProject(testProject),
            Times.Never
        );
    }

    [Fact]
    public async Task UpdateProject_ReturnBadRequest()
    {
        // Global arrange
        long testId = 1;
        ErrorDTO error = null;
        Mock<IRepositoryWrapper> mockRepository = null;
        ProjectController controller = null;
        var projectUpdateDTO = new ProjectUpdateDTO
        {
            Name = "Test Project2",
            Description = "Test Description2"
        };
        // Arrage 1
        mockRepository = new Mock<IRepositoryWrapper>();
        controller = new ProjectController(mockRepository.Object);

        // Act 1
        var result1 = await controller.PutProject(1, null);

        // Assert 1
        var putResult1 = Assert.IsType<BadRequestObjectResult>(result1);
        error = (ErrorDTO)putResult1.Value;
        Assert.Equal("Project object is null", error.Message);


        // Arrage 2
        mockRepository = new Mock<IRepositoryWrapper>();
        mockRepository.Setup(repo => repo.ProjectRepository.GetProjectById(testId))
            .ReturnsAsync(() => null);

        controller = new ProjectController(mockRepository.Object);

        // Act 2
        var result2 = await controller.PutProject(1, projectUpdateDTO);

        // Assert 2
        var createdResult = Assert.IsType<NotFoundObjectResult>(result2);
        error = (ErrorDTO)createdResult.Value;
        Assert.Equal("Project not found", error.Message);
    }

    [Fact]
    public async Task UpdateProject_ReturnOk()
    {
        // Arrage
        long testId = 1;
        var projectUpdateDTO = new ProjectUpdateDTO
        {
            Name = "Test Project",
            Description = "Test Description"
        };
        var now = DateTimeOffset.UtcNow;
        var mockRepository = new Mock<IRepositoryWrapper>();
        var mockProjectRepo = new Mock<IProjectRepository>();

        mockProjectRepo.Setup(r => r.UpdateProject(It.IsAny<Project>()))
        .Callback<Project>(p =>
        {
            p.Id = 1;
            p.Name = projectUpdateDTO.Name;
            p.Description = projectUpdateDTO.Description;
            p.CreatedAt = now;
            p.UpdatedAt = now.AddSeconds(1);
        });
        mockProjectRepo.Setup(r => r.GetProjectById(It.IsAny<long>()))
        .Callback<long>(id => { })
       .ReturnsAsync(new Project
       {
           Id = 1,
           Name = "Project 1",
           Description = null,
           CreatedAt = now
       });

        mockRepository.Setup(r => r.ProjectRepository).Returns(mockProjectRepo.Object);
        mockRepository.Setup(r => r.Save()).Returns(Task.CompletedTask);
        var controller = new ProjectController(mockRepository.Object);

        // Act
        var result = await controller.PutProject(1, projectUpdateDTO);
        Console.WriteLine(result);
        // Assert
        var putResult = Assert.IsType<OkObjectResult>(result);

        var resultProject = (Project)putResult.Value;

        Assert.Equal(testId, resultProject.Id);
        Assert.Equal(projectUpdateDTO.Name, resultProject.Name);
        Assert.Equal(projectUpdateDTO.Description, resultProject.Description);
        Assert.Equal(now, resultProject.CreatedAt);
        Assert.Equal(now.AddSeconds(1), resultProject.UpdatedAt);

        mockRepository.Verify(
            repo => repo.ProjectRepository.GetProjectById(testId),
            Times.Once
        );
        mockRepository.Verify(
            repo => repo.ProjectRepository.UpdateProject(It.IsAny<Project>()),
            Times.Once
        );
        mockRepository.Verify(
            repo => repo.Save(),
            Times.Once
        );
    }
}
#pragma warning restore CS8600, CS8602, CS8625