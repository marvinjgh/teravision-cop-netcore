using Backend.API.Controllers;
using Backend.Service.Contracts;
using Backend.Service.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Backend.Tests;

public class ProjectControllerTest
{
    [Fact]
    public async Task GetProjectById_ReturnsHttpNotFound()
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
        Assert.Equal(testId, notFoundObjectResult.Value);
    }

    [Fact]
    public async Task GetProjectById_ReturnsHttpOk()
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
        var dtoResult = okObjectResult.Value as Project;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        Assert.Equal(dtoTest.Id, dtoResult.Id);
        Assert.Equal(dtoTest.Name, dtoResult.Name);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    }

    [Fact]
    public async Task GetAllProjects_ReturnsHttpOk()
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
    }
}
