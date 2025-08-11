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
        if (project is null)
        {
            return BadRequest("Project object is null");
        }
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

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorDTO>(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> PutProject(long id, [FromBody] ProjectUpdateDTO updateProject)
    {
        if (updateProject is null)
        {

            return BadRequest(new ErrorDTO { Message = "Project object is null" });
        }

        var project = await _repository.Project.GetProjectById(id);

        if (project == null)
        {
            return NotFound(new ErrorDTO { Message = "Project not found" });
        }

        project.Name = updateProject.Name;
        project.Description = updateProject.Description;

        _repository.Project.UpdateProject(project);
        await _repository.Save();

        return NoContent();
    }


    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorDTO>(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> DeleteProject(long id)
    {
        var project = await _repository.Project.GetProjectById(id);

        if (project == null)
        {
            return NotFound(new ErrorDTO { Message = "Project not found" });
        }

        _repository.Project.DeleteProject(project);
        await _repository.Save();

        return NoContent();
    }
}
