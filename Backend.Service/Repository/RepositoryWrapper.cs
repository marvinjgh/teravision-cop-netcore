using Backend.Service.Contracts;

namespace Backend.Service.Repository;

/// <summary>
/// A wrapper for all repositories, providing a single point of access to them.
/// </summary>
/// <param name="repositoryContext">The database context.</param>
public class RepositoryWrapper(RepositoryContext repositoryContext) : IRepositoryWrapper
{

#pragma warning disable CS8618
    private IProjectRepository _project;
    private ITaskRepository _task;
#pragma warning restore CS8618
    /// <summary>
    /// Gets the project repository.
    /// </summary>
    public IProjectRepository ProjectRepository
    {
        get
        {
            _project ??= new ProjectRepository(repositoryContext);
            return _project;
        }
    }
    /// <summary>
    /// Gets the task repository.
    /// </summary>
    public ITaskRepository TaskRepository
    {
        get
        {
            _task ??= new TaskRepository(repositoryContext);
            return _task;
        }
    }
    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    public async Task Save()
    {
        await repositoryContext.SaveChangesAsync();
    }
}
