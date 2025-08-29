using System.ComponentModel;
using Backend.Service.Contracts;
using Backend.Service.DataTransferObjects;
using Backend.Service.Extensions;
using Backend.Service.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[Route("api/task")]
[ApiController]
public class TaskController(IRepositoryWrapper repository) : ControllerBase
{

    [HttpGet("{id}")]
    [ProducesResponseType<TaskDTO>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorDTO>(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> GetTask(long id)
    {
        var task = await repository.TaskRepository.GetTaskById(id);

        if (task == null)
        {
            return NotFound(new ErrorDTO { Message = "Task not found" });
        }

        return Ok(task.ToTaskDto());
    }

    /// <summary>
    /// Return a paginated list of tasks
    /// </summary>
    /// <param name="showAll">Include deleted Tasks in the result</param>
    /// <param name="pageNumber">The current page number (1-based)</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <returns>IActionResult</returns>
    [HttpGet]
    [ProducesResponseType<PaginatedResult<TaskDTO>>(StatusCodes.Status200OK)]
    [Produces("application/json")]
    public async Task<IActionResult> GetAllTasks(
        [Description("Include deleted Tasks in the result")][FromQuery] bool showAll = false,
        [Description("The current page number (1-based)")][FromQuery] int pageNumber = 1,
        [Description("The number of items per page")][FromQuery] int pageSize = 10
    )
    {
        var tasks = await repository.TaskRepository.GetAllTasks(showAll ? null : task =>
            !task.IsDeleted
        );

        PaginatedResult<TaskDTO> paginatedResult = new()
        {
            Items = tasks.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(t => t.ToTaskDto()),
            TotalCount = tasks.Count(),
            PageSize = pageSize,
            CurrentPage = pageNumber
        };

        return Ok(paginatedResult);
    }

    [HttpPost]
    [ProducesResponseType<TaskEntity>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorDTO>(StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    public async Task<IActionResult> PostTask([FromBody] TaskCreateDTO task)
    {
        if (task is null)
        {
            return BadRequest(new ErrorDTO { Message = "Task object is null" });
        }
        if (!ModelState.IsValid)
        {
            // TODO tratar de integrar la lista de errores en el ModelState dentro del ErrorDTO
            return BadRequest(new ErrorDTO { Message = "Invalid model object" });
        }

        var project = await repository.ProjectRepository.GetProjectById(task.ProjectId);
        if (project == null)
        {
            return BadRequest(new ErrorDTO { Message = "Project does not exist" });
        }

        var newTask = new TaskEntity
        {
            Name = task.Name,
            Description = task.Description,
            ProjectId = task.ProjectId
        };

        repository.TaskRepository.CreateTask(newTask);
        await repository.Save();

        return CreatedAtAction(nameof(GetTask), new { id = newTask.Id }, newTask.ToTaskDto());
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorDTO>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorDTO>(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> PutTask(long id, [FromBody] TaskUpdateDTO updateTask)
    {
        if (updateTask is null)
        {
            return BadRequest(new ErrorDTO { Message = "Task object is null" });
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(new ErrorDTO { Message = "Invalid model object" });
        }

        var task = await repository.TaskRepository.GetTaskById(id);

        if (task == null)
        {
            return NotFound(new ErrorDTO { Message = "Task not found" });
        }

        var project = await repository.ProjectRepository.GetProjectById(updateTask.ProjectId);
        if (project == null)
        {
            return NotFound(new ErrorDTO { Message = "Project not found" });
        }

        task.Name = updateTask.Name;
        task.Description = updateTask.Description;
        task.ProjectId = updateTask.ProjectId;

        repository.TaskRepository.UpdateTask(task);
        await repository.Save();

        return Ok(task.ToTaskDto());
    }


    // [HttpDelete("{id}")]
    // [ProducesResponseType(StatusCodes.Status204NoContent)]
    // [ProducesResponseType<ErrorDTO>(StatusCodes.Status404NotFound)]
    // [Produces("application/json")]
    // public async Task<IActionResult> DeleteProject(long id)
    // {
    //     var project = await repository.ProjectRepository.GetProjectById(id);

    //     if (project == null)
    //     {
    //         return NotFound(new ErrorDTO { Message = "Project not found" });
    //     }

    //     repository.ProjectRepository.DeleteProject(project);
    //     await repository.Save();

    //     return NoContent();
    // }
}
