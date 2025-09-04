using System.Linq.Expressions;
using Backend.Service.Contracts;
using Backend.Service.Models;
using Backend.Service.Respository;
using Microsoft.EntityFrameworkCore;

namespace Backend.Service.Repository;

/// <summary>
/// Repository for managing <see cref="TaskEntity"/> instances.
/// </summary>
/// <param name="repositoryContext">The database context.</param>
public class TaskRepository(RepositoryContext repositoryContext) : RepositoryBase<TaskEntity>(repositoryContext), ITaskRepository
{
    /// <summary>
    /// Gets a task by its identifier.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    /// <param name="include">A flag indicating whether to include related entities.</param>
    /// <returns>A task that matches the specified identifier; otherwise, null.</returns>
    public Task<TaskEntity?> GetTaskById(long id, bool include = false)
    {
        if (include)
        {
            return FindByCondition(task => task.Id == id)
                .Include(t => t.Project)
                .FirstOrDefaultAsync();
        }
        else
        {
            return FindByCondition(task => task.Id == id).FirstOrDefaultAsync();
        }
    }
    /// <summary>
    /// Gets all tasks, optionally filtered by a specific expression.
    /// </summary>
    /// <param name="expression">The expression to filter the tasks.</param>
    /// <returns>A collection of tasks.</returns>
    public async Task<IEnumerable<TaskEntity>> GetAllTasks(Expression<Func<TaskEntity, bool>>? expression)
    {
        if (expression is not null)
        {
            return await FindByCondition(expression).ToListAsync();
        }
        return await FindAll().ToListAsync();
    }
    /// <summary>
    /// Creates a new task.
    /// </summary>
    /// <param name="task">The task to create.</param>
    public void CreateTask(TaskEntity task) => Create(task);
    /// <summary>
    /// Updates an existing task.
    /// </summary>
    /// <param name="task">The task to update.</param>
    public void UpdateTask(TaskEntity task) => Update(task);
    /// <summary>
    /// Deletes a task.
    /// </summary>
    /// <param name="task">The task to delete.</param>
    public void DeleteTask(TaskEntity task) => Delete(task);
}
