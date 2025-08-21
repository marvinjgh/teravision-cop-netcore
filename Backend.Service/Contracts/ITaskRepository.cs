using Backend.Service.Models;

namespace Backend.Service.Contracts;

public interface ITaskRepository
{
    Task<IEnumerable<TaskEntity>> GetAllTasks(bool include = false);
    Task<IEnumerable<TaskEntity>> GetAllActiveTasks(bool include = false);
    Task<TaskEntity?> GetTaskById(long taskId, bool include = false);
    void CreateTask(TaskEntity task);
    void UpdateTask(TaskEntity task);
    void DeleteTask(TaskEntity task);
}
