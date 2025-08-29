using System.Linq.Expressions;
using Backend.Service.Contracts;
using Backend.Service.Models;
using Backend.Service.Respository;
using Microsoft.EntityFrameworkCore;

namespace Backend.Service.Repository;

public class TaskRepository(RepositoryContext repositoryContext) : RepositoryBase<TaskEntity>(repositoryContext), ITaskRepository
{
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
    public async Task<IEnumerable<TaskEntity>> GetAllTasks(Expression<Func<TaskEntity, bool>>? expression)
    {
        if (expression is not null)
        {
            return await FindByCondition(expression).ToListAsync();
        }
        return await FindAll().ToListAsync();
    }
    public void CreateTask(TaskEntity task) => Create(task);
    public void UpdateTask(TaskEntity task) => Update(task);
    public void DeleteTask(TaskEntity task) => Delete(task);
}
