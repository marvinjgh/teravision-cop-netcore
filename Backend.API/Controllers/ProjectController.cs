using Backend.Service.Contracts;
using Backend.Service.DataTransferObjects;
using Backend.Service.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[Route("api/project")]
[ApiController]
public class ProjectController : ControllerBase
{
    private readonly IRepositoryWrapper _repository;

    public ProjectController(IRepositoryWrapper repository)
    {
        _repository = repository;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProject(long id)
    {

        var project = await _repository.Project.GetProjectById(id);

        if (project == null)
        {
            return NotFound(id);
        }

        return Ok(project);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProjects()
    {
        var projects = await _repository.Project.GetAllProjects();

        return Ok(projects);
    }

    [HttpPost]
    public async Task<IActionResult> PostProject([FromBody] ProjectCreateDTO project)
    {
        Console.WriteLine(project);
        Console.WriteLine(ModelState.IsValid);

        if (!ModelState.IsValid)
        {
            return BadRequest(new ErrorDTO { Message = "Invalid model object" });
        }

        var newProject = new Project
        {
            Name = project.Name,
            Description = project.Description
        };

        _repository.Project.CreateProject(newProject);
        await _repository.Save();

        return CreatedAtAction(nameof(GetProject), new { id = newProject.Id }, newProject);
    }
}
