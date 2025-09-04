using System.Linq.Expressions;
using Backend.Service.Models;

namespace Backend.Service.Contracts;

/// <summary>
/// Represents the repository for managing tasks.
/// </summary>
public interface ITaskRepository
{
    /// <summary>
    /// Gets all tasks based on an optional expression.
    /// </summary>
    /// <param name="expression">The expression to filter tasks.</param>
    /// <returns>A collection of tasks.</returns>
    Task<IEnumerable<TaskEntity>> GetAllTasks(Expression<Func<TaskEntity, bool>>? expression);

    /// <summary>
    /// Gets a task by its ID.
    /// </summary>
    /// <param name="taskId">The task ID.</param>
    /// <param name="include">A flag to indicate whether to include related entities.</param>
    /// <returns>The task entity if found; otherwise, null.</returns>
    Task<TaskEntity?> GetTaskById(long taskId, bool include = false);

    /// <summary>
    /// Creates a new task.
    /// </summary>
    /// <param name="task">The task to create.</param>
    void CreateTask(TaskEntity task);

    /// <summary>
    /// Updates an existing task.
    /// </summary>
    /// <param name="task">The task to update.</param>
    void UpdateTask(TaskEntity task);

    /// <summary>
    /// Deletes a task.
    /// </summary>
    /// <param name="task">The task to delete.</param>
    void DeleteTask(TaskEntity task);
}
