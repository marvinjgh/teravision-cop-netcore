using Backend.Service.Contracts;
using Backend.Service.DataTransferObjects;
using Backend.Service.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[Route("api/project")]
[ApiController]
public class ProjectController(IRepositoryWrapper repository) : ControllerBase
{
    [HttpGet("{id}")]
    [ProducesResponseType<Project>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorDTO>(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> GetProject(long id)
    {
        var project = await repository.ProjectRepository.GetProjectById(id);

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
        var projects = await repository.ProjectRepository.GetAllProjects();

        return Ok(projects);
    }

    [HttpPost]
    [ProducesResponseType<Project>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorDTO>(StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    public async Task<IActionResult> PostProject([FromBody] ProjectCreateDTO project)
    {
        if (project is null)
        {
            return BadRequest(new ErrorDTO { Message = "Project object is null" });
        }
        if (!ModelState.IsValid)
        {
            // TODO tratar de integrar la lista de errores en el ModelState dentro del ErrorDTO
            return BadRequest(new ErrorDTO { Message = "Invalid model object" });
        }

        var newProject = new Project
        {
            Name = project.Name,
            Description = project.Description
        };

        repository.ProjectRepository.CreateProject(newProject);
        await repository.Save();

        return CreatedAtAction(nameof(GetProject), new { id = newProject.Id }, newProject);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorDTO>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorDTO>(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> PutProject(long id, [FromBody] ProjectUpdateDTO updateProject)
    {
        if (updateProject is null)
        {
            return BadRequest(new ErrorDTO { Message = "Project object is null" });
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(new ErrorDTO { Message = "Invalid model object" });
        }

        var project = await repository.ProjectRepository.GetProjectById(id);

        if (project == null)
        {
            return NotFound(new ErrorDTO { Message = "Project not found" });
        }

        project.Name = updateProject.Name;
        project.Description = updateProject.Description;

        repository.ProjectRepository.UpdateProject(project);
        await repository.Save();

        return Ok(project);
    }


    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorDTO>(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> DeleteProject(long id)
    {
        var project = await repository.ProjectRepository.GetProjectById(id);

        if (project == null)
        {
            return NotFound(new ErrorDTO { Message = "Project not found" });
        }

        repository.ProjectRepository.DeleteProject(project);
        await repository.Save();

        return NoContent();
    }
}
