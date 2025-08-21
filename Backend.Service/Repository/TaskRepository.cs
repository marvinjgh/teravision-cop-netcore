using Backend.Service.Contracts;
using Backend.Service.Models;
using Backend.Service.Respository;
using Microsoft.EntityFrameworkCore;

namespace Backend.Service.Repository;

public class TaskRepository(RepositoryContext repositoryContext) : RepositoryBase<TaskEntity>(repositoryContext), ITaskRepository
{
    public Task<TaskEntity?> GetTaskById(long taskId)
    {
        return FindByCondition(task => task.Id == taskId).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<TaskEntity>> GetAllTasks()
    {
        return await FindAll().ToListAsync();
    }

    public async Task<IEnumerable<TaskEntity>> GetAllActiveTasks()
    {
        return await FindByCondition(task => !task.IsDeleted).ToListAsync();
    }

    public void CreateTask(TaskEntity task) => Create(task);
    public void UpdateTask(TaskEntity task) => Update(task);
    public void DeleteTask(TaskEntity task) => Delete(task);

    public async Task<IEnumerable<TaskEntity>> GetTasksByProjectId(long projectId)
    {
        return await FindByCondition(task => task.ProjectId == projectId).ToListAsync();
    }
}
