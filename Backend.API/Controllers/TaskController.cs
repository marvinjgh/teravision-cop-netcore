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
    /// <summary>
    /// Get a task by its id
    /// </summary>
    /// <param name="id">The id of the task</param>
    /// <returns>The task if found</returns>
    [HttpGet("{id}")]
    [ProducesResponseType<TaskDTO>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorDTO>(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> GetTask([Description("The id of the task")] long id)
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

    /// <summary>
    /// Create a new task
    /// </summary>
    /// <param name="task">The task to create</param>
    /// <returns>The created task</returns>
    [HttpPost]
    [ProducesResponseType<TaskDTO>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorDTO>(StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    public async Task<IActionResult> PostTask([FromBody] TaskRequestDTO task)
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

    /// <summary>
    /// Update an existing task
    /// </summary>
    /// <param name="id">The id of the task to update</param>
    /// <param name="updateTask">The task data to update</param>
    /// <returns>The updated task</returns>
    [HttpPut("{id}")]
    [ProducesResponseType<TaskDTO>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorDTO>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorDTO>(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> PutTask([Description("The id of the task to update")] long id, [FromBody] TaskRequestDTO updateTask)
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

    /// <summary>
    /// Delete a task by its id
    /// </summary>
    /// <param name="id">The id of the task to delete</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorDTO>(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> DeleteTask([Description("The id of the task to delete")] long id)
    {
        var task = await repository.TaskRepository.GetTaskById(id);

        if (task == null)
        {
            return NotFound(new ErrorDTO { Message = "Task not found" });
        }

        task.ProjectId = null;
        repository.TaskRepository.UpdateTask(task);

        repository.TaskRepository.DeleteTask(task);
        await repository.Save();

        return NoContent();
    }

    /// <summary>
    /// Assign a task to a project
    /// </summary>
    /// <param name="taskId">The id of the task</param>
    /// <param name="projectId">The id of the project</param>
    /// <returns>No content</returns>
    [HttpPost("{taskId}/assign/{projectId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorDTO>(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> AssignTaskToProject([Description("The id of the task")] long taskId, [Description("The id of the project")] long projectId)
    {
        var task = await repository.TaskRepository.GetTaskById(taskId);
        if (task == null)
        {
            return NotFound(new ErrorDTO { Message = "Task not found" });
        }

        var project = await repository.ProjectRepository.GetProjectById(projectId);
        if (project == null)
        {
            return NotFound(new ErrorDTO { Message = "Project not found" });
        }

        task.ProjectId = projectId;
        repository.TaskRepository.UpdateTask(task);
        await repository.Save();

        return NoContent();
    }

    /// <summary>
    /// Unassign a task from its project
    /// </summary>
    /// <param name="taskId">The id of the task</param>
    /// <returns>No content</returns>
    [HttpPost("{taskId}/unassign")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorDTO>(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> UnassignTaskFromProject([Description("The id of the task")] long taskId)
    {
        var task = await repository.TaskRepository.GetTaskById(taskId);
        if (task == null)
        {
            return NotFound(new ErrorDTO { Message = "Task not found" });
        }

        task.ProjectId = null;
        repository.TaskRepository.UpdateTask(task);
        await repository.Save();

        return NoContent();
    }
}
