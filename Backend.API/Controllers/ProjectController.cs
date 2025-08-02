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
    [ProducesResponseType<Project>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorDTO>(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> GetProject(long id)
    {
        var project = await _repository.Project.GetProjectById(id);

        if (project == null)
        {
            return NotFound(new ErrorDTO { Message = "Project not found" });
        }

        return Ok(project);
    }

    [HttpGet]
    [ProducesResponseType<IEnumerable<Project>>(StatusCodes.Status200OK)]
    [Produces("application/json")]
    public async Task<IActionResult> GetAllProjects()
    {
        var projects = await _repository.Project.GetAllProjects();

        return Ok(projects);
    }

    [HttpPost]
    [ProducesResponseType<Project>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorDTO>(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> PostProject([FromBody] ProjectCreateDTO project)
    {
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
