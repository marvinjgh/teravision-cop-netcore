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
    public async Task<IEnumerable<TaskEntity>> GetAllTasks(bool include = false)
    {
        if (include)
        {
            return await FindAll().Include(t => t.Project).ToListAsync();
        }
        else
        {
            return await FindAll().ToListAsync();
        }
    }
    public async Task<IEnumerable<TaskEntity>> GetAllActiveTasks(bool include = false)
    {
        if (include)
        {
            return await FindByCondition(task => !task.IsDeleted)
                .Include(t => t.Project)
                .ToListAsync();
        }
        else
        {
            return await FindByCondition(task => !task.IsDeleted).ToListAsync();
        }
    }
    public void CreateTask(TaskEntity task) => Create(task);
    public void UpdateTask(TaskEntity task) => Update(task);
    public void DeleteTask(TaskEntity task) => Delete(task);
}
