using System.ComponentModel;
using Backend.Service.Contracts;
using Backend.Service.DataTransferObjects;
using Backend.Service.Extensions;
using Backend.Service.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[Route("api/project")]
[ApiController]
public class ProjectController(IRepositoryWrapper repository) : ControllerBase
{
    [HttpGet("{id}")]
    [ProducesResponseType<ProjectDTO>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorDTO>(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> GetProject(long id)
    {
        var project = await repository.ProjectRepository.GetProjectById(id, include: true);

        if (project == null)
        {
            return NotFound(new ErrorDTO { Message = "Project not found" });
        }

        return Ok(project.ToProjectDTO());
    }

    /// <summary>
    /// Return a paginated list of projects
    /// </summary>
    /// <param name="showAll">Include deleted Projects in the result</param>
    /// <param name="pageNumber">The current page number (1-based)</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <returns>IActionResult</returns>
    [HttpGet]
    [ProducesResponseType<PaginatedResult<ProjectDTO>>(StatusCodes.Status200OK)]
    [Produces("application/json")]
    public async Task<IActionResult> GetAllProjects(
        [Description("Include deleted Projects in the result")][FromQuery] bool showAll = false,
        [Description("The current page number (1-based)")][FromQuery] int pageNumber = 1,
        [Description("The number of items per page")][FromQuery] int pageSize = 10
    )
    {
        var projects = await repository.ProjectRepository.GetAllProjects(showAll ? null : project =>
            !project.IsDeleted
        );

        PaginatedResult<ProjectDTO> paginatedResult = new()
        {
            Items = projects.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(p => p.ToProjectDTO()),
            TotalCount = projects.Count(),
            PageSize = pageSize,
            CurrentPage = pageNumber
        };

        return Ok(paginatedResult);
    }

    [HttpPost]
    [ProducesResponseType<ProjectDTO>(StatusCodes.Status201Created)]
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

        var newProject = new ProjectEntity
        {
            Name = project.Name,
            Description = project.Description
        };

        repository.ProjectRepository.CreateProject(newProject);
        await repository.Save();

        return CreatedAtAction(nameof(GetProject), new { id = newProject.Id }, newProject.ToProjectDTO());
    }

    [HttpPut("{id}")]
    [ProducesResponseType<ProjectDTO>(StatusCodes.Status200OK)]
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

        return Ok(project.ToProjectDTO());
    }


    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorDTO>(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> DeleteProject(long id)
    {
        var project = await repository.ProjectRepository.GetProjectById(id, include: true);

        if (project == null)
        {
            return NotFound(new ErrorDTO { Message = "Project not found" });
        }

        if (project.Tasks.Count != 0)
        {
            foreach (var task in project.Tasks)
            {
                task.ProjectId = null;
                repository.TaskRepository.UpdateTask(task);
            }
        }

        repository.ProjectRepository.DeleteProject(project);
        await repository.Save();

        return NoContent();
    }

    [HttpGet("{id}/tasks")]
    [ProducesResponseType<PaginatedResult<TaskDTO>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorDTO>(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> GetProjectTasks(
        [Description("Project id")] long id,
        [Description("Page number")][FromQuery] int pageNumber = 1,
        [Description("Page size")][FromQuery] int pageSize = 10
    )
    {
        var project = await repository.ProjectRepository.GetProjectById(id, true);
        if (project == null)
        {
            return NotFound(new ErrorDTO { Message = "Project not found" });
        }

        PaginatedResult<TaskDTO> paginatedResult = new()
        {
            Items = project.Tasks.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(t => t.ToTaskDto()),
            TotalCount = project.Tasks.Count(),
            PageSize = pageSize,
            CurrentPage = pageNumber
        };

        return Ok(paginatedResult);
    }
}
