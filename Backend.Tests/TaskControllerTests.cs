using Backend.API.Controllers;
using Backend.Service.Contracts;
using Backend.Service.DataTransferObjects;
using Backend.Service.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Backend.Tests;
#pragma warning disable CS8600, CS8602, CS8625
public class TaskControllerTests
{
    [Fact]
    public async Task GetTaskById_ReturnsNotFound()
    {
        // Arrange
        long testId = 1;

        var mockRepo = new Mock<IRepositoryWrapper>();
        mockRepo.Setup(repo => repo.TaskRepository.GetTaskById(testId))
            .ReturnsAsync((TaskEntity?)null);

        var controller = new TaskController(mockRepo.Object);

        //Act
        var result = await controller.GetTask(testId);

        //Assert
        var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
        ErrorDTO error = (ErrorDTO)notFoundObjectResult.Value;
        Assert.Equal("Task not found", error.Message);

        mockRepo.Verify(
            repo => repo.TaskRepository.GetTaskById(testId),
            Times.Once
        );
    }

    [Fact]
    public async Task GetTaskById_ReturnsOk()
    {
        // Arrange
        long testId = 1;
        var now = DateTimeOffset.UtcNow;
        var mockRepo = new Mock<IRepositoryWrapper>();
        var taskEntity = new TaskEntity
        {
            Id = 1,
            Name = "Task 1",
            Description = "Description of Task 1",
            CreatedAt = now,
            UpdatedAt = now
        };

        mockRepo.Setup(repo => repo.TaskRepository.GetTaskById(testId))
            .ReturnsAsync(taskEntity);

        var controller = new TaskController(mockRepo.Object);

        //Act
        var result = await controller.GetTask(testId);

        //Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(result);
        var taskResult = (TaskEntity)okObjectResult.Value;

        Assert.Equal(taskEntity.Id, taskResult.Id);
        Assert.Equal(taskEntity.Name, taskResult.Name);
        Assert.Equal(taskEntity.Description, taskResult.Description);
        Assert.Equal(taskEntity.CreatedAt, taskResult.CreatedAt);
        Assert.Equal(taskEntity.UpdatedAt, taskResult.UpdatedAt);


        mockRepo.Verify(
            repo => repo.TaskRepository.GetTaskById(testId),
            Times.Once
        );
    }

    [Fact]
    public async Task GetAllTasks_ReturnsOk()
    {
        // Arrange
        var mockRepo = new Mock<IRepositoryWrapper>();
        mockRepo.Setup(repo => repo.TaskRepository.GetAllTasks())
            .ReturnsAsync(new List<TaskEntity>
            {
                 new() { Id = 1, Name = "Task 1", Description = "Description 1" },
                 new() { Id = 2, Name = "Task 2", Description = "Description 2" }
            });

        var controller = new TaskController(mockRepo.Object);

        //Act
        var result = await controller.GetAllTasks();

        //Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(result);
        var tasks = Assert.IsAssignableFrom<List<TaskEntity>>(okObjectResult.Value);
        Assert.Equal(2, tasks.Count);

        mockRepo.Verify(
           repo => repo.TaskRepository.GetAllTasks(),
           Times.Once
        );
    }

    [Fact]
    public async Task PostTask_ReturnsBadRequest()
    {
        // Arrange: create mocks and simulate new project Id assignment
        var mockRepository = new Mock<IRepositoryWrapper>();

        var controller = new TaskController(mockRepository.Object);
        var taskCreateDTO = new TaskCreateDTO();

        controller.ModelState.AddModelError("Name", "The Name field is required.");

        // Act
        var result = await controller.PostTask(taskCreateDTO);

        // Assert
        var createdResult = Assert.IsType<BadRequestObjectResult>(result);
        ErrorDTO error = (ErrorDTO)createdResult.Value;
        Assert.Equal("Invalid model object", error.Message);

        mockRepository.Verify(
            repo => repo.TaskRepository.CreateTask(It.IsAny<TaskEntity>()),
            Times.Never
        );
        mockRepository.Verify(
            repo => repo.Save(),
            Times.Never
        );
    }

    [Fact]
    public async Task PostTask_ReturnsCreatedAtAction()
    {
        // Arrange: create mocks and simulate new project Id assignment
        long testId = 1;
        var now = DateTimeOffset.UtcNow;
        var taskCreateDTO = new TaskCreateDTO
        {
            Name = "Test Task",
            Description = "Test Description"
        };

        var mockRepository = new Mock<IRepositoryWrapper>();
        var mockTaskRepo = new Mock<ITaskRepository>();
        mockTaskRepo.Setup(r => r.CreateTask(It.IsAny<TaskEntity>()))
            .Callback<TaskEntity>(p =>
            {
                p.Id = 1;
                p.Name = taskCreateDTO.Name;
                p.Description = taskCreateDTO.Description;
                p.CreatedAt = now;
                p.UpdatedAt = now;
            });
        mockRepository.Setup(r => r.TaskRepository).Returns(mockTaskRepo.Object);
        mockRepository.Setup(r => r.Save()).Returns(Task.CompletedTask);

        var controller = new TaskController(mockRepository.Object);

        // Act
        var result = await controller.PostTask(taskCreateDTO);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.NotNull(createdResult.RouteValues);
        Assert.Equal(testId, createdResult.RouteValues["id"]);

        var resultTask = (TaskEntity)createdResult.Value;

        Assert.Equal(testId, resultTask.Id);
        Assert.Equal(taskCreateDTO.Name, resultTask.Name);
        Assert.Equal(taskCreateDTO.Description, resultTask.Description);
        Assert.Equal(now, resultTask.CreatedAt);
        Assert.Equal(now, resultTask.UpdatedAt);

        // Opcional: Verificar que el método Save nunca fue llamado.
        mockRepository.Verify(
            repo => repo.TaskRepository.CreateTask(It.IsAny<TaskEntity>()),
            Times.Once
        );
        mockRepository.Verify(
            repo => repo.Save(),
            Times.Once
        );
    }

    // [Fact]
    // public async Task DeleteProject_ReturnsOk()
    // {
    //     //Arrange
    //     long testId = 1;
    //     Project testProject = new() { Id = testId };

    //     var mockRepo = new Mock<IRepositoryWrapper>();
    //     mockRepo.Setup(repo => repo.ProjectRepository.GetProjectById(testId))
    //         .ReturnsAsync(testProject);

    //     var controller = new ProjectController(mockRepo.Object);

    //     //Act
    //     var result = await controller.DeleteProject(testId);

    //     //Assert
    //     var okObjectResult = Assert.IsType<NoContentResult>(result);
    //     Assert.NotNull(okObjectResult);
    //     mockRepo.Verify(
    //         repo => repo.ProjectRepository.DeleteProject(testProject),
    //         Times.Once
    //     );
    //     mockRepo.Verify(
    //         repo => repo.Save(),
    //         Times.Once
    //     );
    // }

    // [Fact]
    // public async Task DeleteProject_ReturnsNotFound()
    // {
    //     // Arrange
    //     long testId = 1;
    //     Project testProject = new() { Id = testId };

    //     var mockRepo = new Mock<IRepositoryWrapper>();
    //     mockRepo.Setup(repo => repo.ProjectRepository.GetProjectById(testId))
    //         .ReturnsAsync((Project?)null);

    //     var controller = new ProjectController(mockRepo.Object);

    //     //Act
    //     var result = await controller.DeleteProject(testId);

    //     //Assert
    //     var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
    //     ErrorDTO error = (ErrorDTO)notFoundObjectResult.Value;
    //     Assert.Equal("Project not found", error.Message);

    //     mockRepo.Verify(
    //         repo => repo.ProjectRepository.DeleteProject(testProject),
    //         Times.Never
    //     );
    // }

    // [Fact]
    // public async Task UpdateProject_ReturnBadRequest()
    // {
    //     // Global arrange
    //     long testId = 1;
    //     ErrorDTO error = null;
    //     Mock<IRepositoryWrapper> mockRepository = null;
    //     ProjectController controller = null;
    //     var projectUpdateDTO = new ProjectUpdateDTO
    //     {
    //         Name = "Test Project2",
    //         Description = "Test Description2"
    //     };
    //     // Arrage 1
    //     mockRepository = new Mock<IRepositoryWrapper>();
    //     controller = new ProjectController(mockRepository.Object);

    //     // Act 1
    //     var result1 = await controller.PutProject(1, null);

    //     // Assert 1
    //     var putResult1 = Assert.IsType<BadRequestObjectResult>(result1);
    //     error = (ErrorDTO)putResult1.Value;
    //     Assert.Equal("Project object is null", error.Message);


    //     // Arrage 2
    //     mockRepository = new Mock<IRepositoryWrapper>();
    //     mockRepository.Setup(repo => repo.ProjectRepository.GetProjectById(testId))
    //         .ReturnsAsync(() => null);

    //     controller = new ProjectController(mockRepository.Object);

    //     // Act 2
    //     var result2 = await controller.PutProject(1, projectUpdateDTO);

    //     // Assert 2
    //     var createdResult = Assert.IsType<NotFoundObjectResult>(result2);
    //     error = (ErrorDTO)createdResult.Value;
    //     Assert.Equal("Project not found", error.Message);
    // }

    // [Fact]
    // public async Task UpdateProject_ReturnOk()
    // {
    //     // Arrage
    //     long testId = 1;
    //     var projectUpdateDTO = new ProjectUpdateDTO
    //     {
    //         Name = "Test Project",
    //         Description = "Test Description"
    //     };
    //     var now = DateTimeOffset.UtcNow;
    //     var mockRepository = new Mock<IRepositoryWrapper>();
    //     var mockProjectRepo = new Mock<IProjectRepository>();

    //     mockProjectRepo.Setup(r => r.UpdateProject(It.IsAny<Project>()))
    //     .Callback<Project>(p =>
    //     {
    //         p.Id = 1;
    //         p.Name = projectUpdateDTO.Name;
    //         p.Description = projectUpdateDTO.Description;
    //         p.CreatedAt = now;
    //     });
    //     mockProjectRepo.Setup(r => r.GetProjectById(It.IsAny<long>()))
    //     .Callback<long>(id => { })
    //    .ReturnsAsync(new Project
    //    {
    //        Id = 1,
    //        Name = "Project 1",
    //        Description = null,
    //        CreatedAt = now
    //    });

    //     mockRepository.Setup(r => r.ProjectRepository).Returns(mockProjectRepo.Object);
    //     mockRepository.Setup(r => r.Save()).Returns(Task.CompletedTask);
    //     var controller = new ProjectController(mockRepository.Object);

    //     // Act
    //     var result = await controller.PutProject(1, projectUpdateDTO);
    //     Console.WriteLine(result);
    //     // Assert
    //     var putResult = Assert.IsType<NoContentResult>(result);
    //     mockRepository.Verify(
    //         repo => repo.ProjectRepository.GetProjectById(testId),
    //         Times.Once
    //     );
    //     mockRepository.Verify(
    //         repo => repo.ProjectRepository.UpdateProject(It.IsAny<Project>()),
    //         Times.Once
    //     );
    //     mockRepository.Verify(
    //         repo => repo.Save(),
    //         Times.Once
    //     );
    // }
}
#pragma warning restore CS8600, CS8602, CS8625