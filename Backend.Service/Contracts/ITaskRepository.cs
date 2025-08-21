using Backend.Service.Models;

namespace Backend.Service.Contracts;

public interface ITaskRepository
{
    Task<IEnumerable<TaskEntity>> GetAllTasks();
    Task<IEnumerable<TaskEntity>> GetAllActiveTasks();
    Task<TaskEntity?> GetTaskById(long taskId);

    Task<IEnumerable<TaskEntity>> GetTasksByProjectId(long projectId);
    void CreateTask(TaskEntity task);
    void UpdateTask(TaskEntity task);
    void DeleteTask(TaskEntity task);
}
