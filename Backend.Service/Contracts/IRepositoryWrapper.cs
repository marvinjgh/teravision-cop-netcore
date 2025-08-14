namespace Backend.Service.Contracts;

public interface IRepositoryWrapper
{
    IProjectRepository ProjectRepository { get; }
    ITaskRepository TaskRepository { get; }
    Task Save();
}
