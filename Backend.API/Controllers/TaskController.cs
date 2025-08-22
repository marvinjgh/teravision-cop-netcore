using Backend.Service.Contracts;
using Backend.Service.DataTransferObjects;
using Backend.Service.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[Route("api/task")]
[ApiController]
public class TaskController(IRepositoryWrapper repository) : ControllerBase
{

    [HttpGet("{id}")]
    [ProducesResponseType<TaskEntity>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorDTO>(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> GetTask(long id)
    {
        var task = await repository.TaskRepository.GetTaskById(id);

        if (task == null)
        {
            return NotFound(new ErrorDTO { Message = "Task not found" });
        }

        return Ok(task);
    }

    [HttpGet]
    [ProducesResponseType<IEnumerable<TaskEntity>>(StatusCodes.Status200OK)]
    [Produces("application/json")]
    public async Task<IActionResult> GetAllTasks([FromQuery] bool showAll = false)
    {
        var tasks = await (showAll ? repository.TaskRepository.GetAllTasks() : repository.TaskRepository.GetAllActiveTasks());

        return Ok(tasks);
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

        return CreatedAtAction(nameof(GetTask), new { id = newTask.Id }, newTask);
    }

    // [HttpPut("{id}")]
    // [ProducesResponseType(StatusCodes.Status200OK)]
    // [ProducesResponseType<ErrorDTO>(StatusCodes.Status400BadRequest)]
    // [ProducesResponseType<ErrorDTO>(StatusCodes.Status404NotFound)]
    // [Produces("application/json")]
    // public async Task<IActionResult> PutProject(long id, [FromBody] ProjectUpdateDTO updateProject)
    // {
    //     if (updateProject is null)
    //     {
    //         return BadRequest(new ErrorDTO { Message = "Project object is null" });
    //     }
    //     if (!ModelState.IsValid)
    //     {
    //         return BadRequest(new ErrorDTO { Message = "Invalid model object" });
    //     }

    //     var project = await repository.ProjectRepository.GetProjectById(id);

    //     if (project == null)
    //     {
    //         return NotFound(new ErrorDTO { Message = "Project not found" });
    //     }

    //     project.Name = updateProject.Name;
    //     project.Description = updateProject.Description;

    //     repository.ProjectRepository.UpdateProject(project);
    //     await repository.Save();

    //     return Ok(project);
    // }


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
