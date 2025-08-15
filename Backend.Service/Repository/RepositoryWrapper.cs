using Backend.Service.Contracts;

namespace Backend.Service.Repository;

public class RepositoryWrapper(RepositoryContext repositoryContext) : IRepositoryWrapper
{

#pragma warning disable CS8618
    private IProjectRepository _project;
    private ITaskRepository _task;
#pragma warning restore CS8618
    public IProjectRepository ProjectRepository
    {
        get
        {
            _project ??= new ProjectRepository(repositoryContext);
            return _project;
        }
    }
    public ITaskRepository TaskRepository
    {
        get
        {
            _task ??= new TaskRepository(repositoryContext);
            return _task;
        }
    }
    public async Task Save()
    {
        await repositoryContext.SaveChangesAsync();
    }
}
