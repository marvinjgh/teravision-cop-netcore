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
        mockRepo.Setup(repo => repo.TaskRepository.GetTaskById(testId, false))
            .ReturnsAsync((TaskEntity?)null);

        var controller = new TaskController(mockRepo.Object);

        //Act
        var result = await controller.GetTask(testId);

        //Assert
        var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
        ErrorDTO error = (ErrorDTO)notFoundObjectResult.Value;
        Assert.Equal("Task not found", error.Message);

        mockRepo.Verify(
            repo => repo.TaskRepository.GetTaskById(testId, false),
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

        mockRepo.Setup(repo => repo.TaskRepository.GetTaskById(testId, false))
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
            repo => repo.TaskRepository.GetTaskById(testId, false),
            Times.Once
        );
    }

    [Fact]
    public async Task GetAllTasks_ReturnsOk()
    {
        // Arrange
        var mockRepo = new Mock<IRepositoryWrapper>();
        mockRepo.Setup(repo => repo.TaskRepository.GetAllTasks(false))
            .ReturnsAsync(new List<TaskEntity>
            {
                 new() { Id = 1, Name = "Task 1", Description = "Description 1", IsDeleted = false },
                 new() { Id = 2, Name = "Task 2", Description = "Description 2", IsDeleted = true }
            });

        var controller = new TaskController(mockRepo.Object);

        //Act
        var result = await controller.GetAllTasks(showAll: true);

        //Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(result);
        var tasks = Assert.IsAssignableFrom<List<TaskEntity>>(okObjectResult.Value);
        Assert.Equal(2, tasks.Count);

        mockRepo.Verify(
           repo => repo.TaskRepository.GetAllTasks(false),
           Times.Once
        );
    }

    [Fact]
    public async Task GetAllActiveTasks_ReturnsOk()
    {
        // Arrange
        var mockRepo = new Mock<IRepositoryWrapper>();
        mockRepo.Setup(repo => repo.TaskRepository.GetAllTasks(false))
            .ReturnsAsync(new List<TaskEntity>
            {
                 new() { Id = 1, Name = "Task 1", Description = "Description 1", IsDeleted = false },
                 new() { Id = 2, Name = "Task 2", Description = "Description 2", IsDeleted = true }
            }.Where(task => !task.IsDeleted).ToList());

        var controller = new TaskController(mockRepo.Object);

        //Act
        var result = await controller.GetAllTasks(showAll: true);

        //Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(result);
        var tasks = Assert.IsAssignableFrom<List<TaskEntity>>(okObjectResult.Value);
        Assert.Single(tasks);

        mockRepo.Verify(
           repo => repo.TaskRepository.GetAllTasks(false),
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
            Description = "Test Description",
            ProjectId = testId
        };

        var mockRepository = new Mock<IRepositoryWrapper>();
        var mockTaskRepo = new Mock<ITaskRepository>();
        var mockProjectRepo = new Mock<IProjectRepository>();

        mockProjectRepo.Setup(r => r.GetProjectById(testId, false))
        .ReturnsAsync(new Project { Id = testId, Name = "Project 1" });

        mockTaskRepo.Setup(r => r.CreateTask(It.IsAny<TaskEntity>()))
            .Callback<TaskEntity>(p =>
            {
                p.Id = 1;
                p.Name = taskCreateDTO.Name;
                p.Description = taskCreateDTO.Description;
                p.ProjectId = taskCreateDTO.ProjectId;
                p.CreatedAt = now;
                p.UpdatedAt = now;
            });
        mockRepository.Setup(r => r.TaskRepository).Returns(mockTaskRepo.Object);
        mockRepository.Setup(r => r.ProjectRepository).Returns(mockProjectRepo.Object);
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
        Assert.Equal(taskCreateDTO.ProjectId, resultTask.ProjectId);
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
        mockRepository.Verify(
        repo => repo.ProjectRepository.GetProjectById(testId, false),
        Times.Once
    );
    }

    [Fact]
    public async Task PostTask_ReturnsBadRequest_ProjectDoesNotExist()
    {
        // Arrange
        long invalidProjectId = 0;
        var taskCreateDTO = new TaskCreateDTO
        {
            Name = "Test Task",
            Description = "Test Description",
            ProjectId = invalidProjectId
        };

        var mockRepository = new Mock<IRepositoryWrapper>();
        var mockProjectRepo = new Mock<IProjectRepository>();
        var mockTaskRepo = new Mock<ITaskRepository>();

        mockProjectRepo.Setup(r => r.GetProjectById(invalidProjectId, false))
            .ReturnsAsync((Project?)null);

        mockRepository.Setup(r => r.ProjectRepository).Returns(mockProjectRepo.Object);
        mockRepository.Setup(r => r.TaskRepository).Returns(mockTaskRepo.Object);

        var controller = new TaskController(mockRepository.Object);

        // Act
        var result = await controller.PostTask(taskCreateDTO);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var error = Assert.IsType<ErrorDTO>(badRequestResult.Value);
        Assert.Equal("Project does not exist", error.Message);

        mockRepository.Verify(
            repo => repo.ProjectRepository.GetProjectById(invalidProjectId, false),
            Times.Once
        );
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
    public async Task UpdateTask_BadRequest()
    {
        // Global arrange
        ErrorDTO error;
        Mock<IRepositoryWrapper> mockRepository;
        TaskController controller;
        var taskUpdateDTO = new TaskUpdateDTO();

        // Arrage 1
        mockRepository = new Mock<IRepositoryWrapper>();
        controller = new TaskController(mockRepository.Object);

        // Act 1
        var result1 = await controller.PutTask(1, null);

        // Assert 1
        var putResult1 = Assert.IsType<BadRequestObjectResult>(result1);
        error = (ErrorDTO)putResult1.Value;
        Assert.Equal("Task object is null", error.Message);

        // Arrage 2
        mockRepository = new Mock<IRepositoryWrapper>();
        controller = new TaskController(mockRepository.Object);
        controller.ModelState.AddModelError("Name", "The Name field is required.");

        // Act 2
        var result2 = await controller.PutTask(1, taskUpdateDTO);

        // Assert 2
        var putResult2 = Assert.IsType<BadRequestObjectResult>(result2);
        error = (ErrorDTO)putResult2.Value;
        Assert.Equal("Invalid model object", error.Message);
    }

    [Fact]
    public async Task UpdateTask_NotFound()
    {
        // Global arrange
        long testId = 1;
        ErrorDTO error;
        Mock<IRepositoryWrapper> mockRepository;
        TaskController controller;
        var taskUpdateDTO = new TaskUpdateDTO
        {
            Name = "T1",
            Description = "",
            ProjectId = 0
        };

        // Arrage 1
        mockRepository = new Mock<IRepositoryWrapper>();
        mockRepository.Setup(repo => repo.TaskRepository.GetTaskById(testId, false))
            .ReturnsAsync(() => null);
        controller = new TaskController(mockRepository.Object);

        // Act 1
        var result1 = await controller.PutTask(1, taskUpdateDTO);

        // Assert 1
        var putResult1 = Assert.IsType<NotFoundObjectResult>(result1);
        error = (ErrorDTO)putResult1.Value;
        Assert.Equal("Task not found", error.Message);

        // Arrage 2
        mockRepository = new Mock<IRepositoryWrapper>();
        mockRepository.Setup(repo => repo.TaskRepository.GetTaskById(testId, false))
            .ReturnsAsync(() => new TaskEntity());
        mockRepository.Setup(repo => repo.ProjectRepository.GetProjectById(taskUpdateDTO.ProjectId, false))
            .ReturnsAsync(() => null);
        controller = new TaskController(mockRepository.Object);


        // Act 2
        var result2 = await controller.PutTask(1, taskUpdateDTO);

        // Assert 2
        var putResult2 = Assert.IsType<NotFoundObjectResult>(result2);
        error = (ErrorDTO)putResult2.Value;
        Assert.Equal("Project not found", error.Message);
    }

    [Fact]
    public async Task UpdateTask_ReturnOk()
    {
        // Arrage
        long testId = 1;
        var taskUpdateDTO = new TaskUpdateDTO
        {
            Name = "Test task",
            Description = "Test Description",
            ProjectId = 1
        };
        var now = DateTimeOffset.UtcNow;
        var project = new Project { Id = testId, Name = "Project 1", IsDeleted = false, CreatedAt = now, UpdatedAt = now };
        var task = new TaskEntity { Id = testId, Name = "Task 1", IsDeleted = false, CreatedAt = now, UpdatedAt = now };
        var mockRepository = new Mock<IRepositoryWrapper>();
        var mockProjectRepo = new Mock<IProjectRepository>();
        var mockTaskRepo = new Mock<ITaskRepository>();

        mockProjectRepo.Setup(r => r.GetProjectById(testId, false)).ReturnsAsync(project);
        mockTaskRepo.Setup(r => r.GetTaskById(testId, false)).ReturnsAsync(task);
        mockRepository.Setup(r => r.ProjectRepository).Returns(mockProjectRepo.Object);
        mockRepository.Setup(r => r.TaskRepository).Returns(mockTaskRepo.Object);
        mockTaskRepo.Setup(r => r.UpdateTask(It.IsAny<TaskEntity>())).Callback<TaskEntity>(t =>
        {
            t.Name = taskUpdateDTO.Name;
            t.Description = taskUpdateDTO.Description;
            t.UpdatedAt = now.AddMinutes(1);
            t.ProjectId = testId;
        });
        mockRepository.Setup(r => r.Save()).Returns(Task.CompletedTask);

        var controller = new TaskController(mockRepository.Object);

        // Act
        var result = await controller.PutTask(1, taskUpdateDTO);

        // Assert
        var putResult = Assert.IsType<OkObjectResult>(result);

        var resultTask = (TaskEntity)putResult.Value;

        Assert.Equal(resultTask.Id, task.Id);
        Assert.Equal(resultTask.Name, taskUpdateDTO.Name);
        Assert.Equal(resultTask.Description, taskUpdateDTO.Description);
        Assert.Equal(resultTask.CreatedAt, task.CreatedAt);
        Assert.Equal(resultTask.UpdatedAt, now.AddMinutes(1));
        Assert.Equal(resultTask.ProjectId, project.Id);

        mockProjectRepo.Verify(
            repo => repo.GetProjectById(It.Is<long>(id => id == testId), false),
            Times.Once
        );
        mockTaskRepo.Verify(
            repo => repo.GetTaskById(It.Is<long>(id => id == testId), false),
            Times.Once
        );
        mockTaskRepo.Verify(r => r.UpdateTask(It.IsAny<TaskEntity>()), Times.Once);
        mockRepository.Verify(r => r.Save(), Times.Once);
    }

    [Fact]
    public async Task DeleteTask_ReturnsNotFound()
    {
        // Arrange
        long testId = 1;

        var mockRepo = new Mock<IRepositoryWrapper>();
        mockRepo.Setup(repo => repo.TaskRepository.GetTaskById(testId, false))
            .ReturnsAsync((TaskEntity?)null);

        var controller = new TaskController(mockRepo.Object);

        //Act
        var result = await controller.DeleteTask(testId);

        //Assert
        var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
        ErrorDTO error = (ErrorDTO)notFoundObjectResult.Value;
        Assert.Equal("Task not found", error.Message);

        mockRepo.Verify(
            repo => repo.TaskRepository.GetTaskById(testId, false),
            Times.Once
        );
    }

    [Fact]
    public async Task DeleteProject_ReturnsOk()
    {
        // Arrange
        long testId = 1;
        var now = DateTimeOffset.UtcNow;
        var taskEntity = new TaskEntity
        {
            Id = 1,
            Name = "Task 1",
            Description = "Description of Task 1",
            CreatedAt = now,
            UpdatedAt = now
        };

        var mockRepo = new Mock<IRepositoryWrapper>();
        mockRepo.Setup(repo => repo.TaskRepository.GetTaskById(testId, false))
            .ReturnsAsync(taskEntity);
        mockRepo.Setup(repo => repo.TaskRepository.DeleteTask(taskEntity));
        mockRepo.Setup(repo => repo.Save()).Returns(Task.CompletedTask);

        var controller = new TaskController(mockRepo.Object);

        //Act
        var result = await controller.DeleteTask(testId);

        //Assert
        Assert.IsType<NoContentResult>(result);

        mockRepo.Verify(
            repo => repo.TaskRepository.GetTaskById(testId, false),
            Times.Once
        );
        mockRepo.Verify(
            repo => repo.TaskRepository.DeleteTask(taskEntity),
            Times.Once
        );
        mockRepo.Verify(
            repo => repo.Save(),
            Times.Once
        );
    }

    [Fact]
    public async Task AssignTaskToProject_ReturnsNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        long taskId = 1, projectId = 2;
        var mockRepository = new Mock<IRepositoryWrapper>();
        var mockTaskRepo = new Mock<ITaskRepository>();
        var mockProjectRepo = new Mock<IProjectRepository>();

        mockTaskRepo.Setup(r => r.GetTaskById(taskId, false)).ReturnsAsync((TaskEntity?)null);
        mockRepository.Setup(r => r.TaskRepository).Returns(mockTaskRepo.Object);
        mockRepository.Setup(r => r.ProjectRepository).Returns(mockProjectRepo.Object);

        var controller = new TaskController(mockRepository.Object);

        // Act
        var result = await controller.AssignTaskToProject(taskId, projectId);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var error = Assert.IsType<ErrorDTO>(notFound.Value);
        Assert.Equal("Task not found", error.Message);

        mockTaskRepo.Verify(r => r.GetTaskById(taskId, false), Times.Once);
        mockProjectRepo.Verify(r => r.GetProjectById(It.IsAny<long>(), false), Times.Never);
    }

    [Fact]
    public async Task AssignTaskToProject_ReturnsNotFound_WhenProjectDoesNotExist()
    {
        // Arrange
        long taskId = 1, projectId = 2;
        var task = new TaskEntity { Id = taskId, Name = "Task", ProjectId = 0 };
        var mockRepository = new Mock<IRepositoryWrapper>();
        var mockTaskRepo = new Mock<ITaskRepository>();
        var mockProjectRepo = new Mock<IProjectRepository>();

        mockTaskRepo.Setup(r => r.GetTaskById(taskId, false)).ReturnsAsync(task);
        mockProjectRepo.Setup(r => r.GetProjectById(projectId, false)).ReturnsAsync((Project?)null);
        mockRepository.Setup(r => r.TaskRepository).Returns(mockTaskRepo.Object);
        mockRepository.Setup(r => r.ProjectRepository).Returns(mockProjectRepo.Object);

        var controller = new TaskController(mockRepository.Object);

        // Act
        var result = await controller.AssignTaskToProject(taskId, projectId);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var error = Assert.IsType<ErrorDTO>(notFound.Value);
        Assert.Equal("Project not found", error.Message);

        mockTaskRepo.Verify(r => r.GetTaskById(taskId, false), Times.Once);
        mockProjectRepo.Verify(r => r.GetProjectById(projectId, false), Times.Once);
    }

    [Fact]
    public async Task AssignTaskToProject_ReturnsNoContent_WhenSuccess()
    {
        // Arrange
        long taskId = 1, projectId = 2;
        var task = new TaskEntity { Id = taskId, Name = "Task", ProjectId = 0 };
        var project = new Project { Id = projectId, Name = "Project" };
        var mockRepository = new Mock<IRepositoryWrapper>();
        var mockTaskRepo = new Mock<ITaskRepository>();
        var mockProjectRepo = new Mock<IProjectRepository>();

        mockTaskRepo.Setup(r => r.GetTaskById(taskId, false)).ReturnsAsync(task);
        mockProjectRepo.Setup(r => r.GetProjectById(projectId, false)).ReturnsAsync(project);
        mockRepository.Setup(r => r.TaskRepository).Returns(mockTaskRepo.Object);
        mockRepository.Setup(r => r.ProjectRepository).Returns(mockProjectRepo.Object);
        mockTaskRepo.Setup(r => r.UpdateTask(task));
        mockRepository.Setup(r => r.Save()).Returns(Task.CompletedTask);

        var controller = new TaskController(mockRepository.Object);

        // Act
        var result = await controller.AssignTaskToProject(taskId, projectId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.Equal(projectId, task.ProjectId);

        mockTaskRepo.Verify(r => r.GetTaskById(taskId, false), Times.Once);
        mockProjectRepo.Verify(r => r.GetProjectById(projectId, false), Times.Once);
        mockTaskRepo.Verify(r => r.UpdateTask(task), Times.Once);
        mockRepository.Verify(r => r.Save(), Times.Once);
    }

    [Fact]
    public async Task UnassignTaskFromProject_ReturnsNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        long taskId = 1;
        var mockRepository = new Mock<IRepositoryWrapper>();
        var mockTaskRepo = new Mock<ITaskRepository>();

        mockTaskRepo.Setup(r => r.GetTaskById(taskId, false)).ReturnsAsync((TaskEntity?)null);
        mockRepository.Setup(r => r.TaskRepository).Returns(mockTaskRepo.Object);

        var controller = new TaskController(mockRepository.Object);

        // Act
        var result = await controller.UnassignTaskFromProject(taskId);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var error = Assert.IsType<ErrorDTO>(notFound.Value);
        Assert.Equal("Task not found", error.Message);

        mockTaskRepo.Verify(r => r.GetTaskById(taskId, false), Times.Once);
    }

    [Fact]
    public async Task UnassignTaskFromProject_ReturnsNoContent_WhenSuccess()
    {
        // Arrange
        long taskId = 1;
        var task = new TaskEntity { Id = taskId, Name = "Task", ProjectId = 2 };
        var mockRepository = new Mock<IRepositoryWrapper>();
        var mockTaskRepo = new Mock<ITaskRepository>();

        mockTaskRepo.Setup(r => r.GetTaskById(taskId, false)).ReturnsAsync(task);
        mockRepository.Setup(r => r.TaskRepository).Returns(mockTaskRepo.Object);
        mockTaskRepo.Setup(r => r.UpdateTask(task));
        mockRepository.Setup(r => r.Save()).Returns(Task.CompletedTask);

        var controller = new TaskController(mockRepository.Object);

        // Act
        var result = await controller.UnassignTaskFromProject(taskId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.Null(task.ProjectId);

        mockTaskRepo.Verify(r => r.GetTaskById(taskId, false), Times.Once);
        mockTaskRepo.Verify(r => r.UpdateTask(task), Times.Once);
        mockRepository.Verify(r => r.Save(), Times.Once);
    }


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


}
#pragma warning restore CS8600, CS8602, CS8625