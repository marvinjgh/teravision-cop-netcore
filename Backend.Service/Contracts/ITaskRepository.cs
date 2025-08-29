using System.Linq.Expressions;
using Backend.Service.Models;

namespace Backend.Service.Contracts;

public interface ITaskRepository
{
    Task<IEnumerable<TaskEntity>> GetAllTasks(Expression<Func<TaskEntity, bool>>? expression);
    Task<TaskEntity?> GetTaskById(long taskId, bool include = false);
    void CreateTask(TaskEntity task);
    void UpdateTask(TaskEntity task);
    void DeleteTask(TaskEntity task);
}
