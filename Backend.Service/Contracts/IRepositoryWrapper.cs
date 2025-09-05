namespace Backend.Service.Contracts;

/// <summary>
/// Represents a wrapper for all repositories, providing a single point of access.
/// </summary>
public interface IRepositoryWrapper
{
    /// <summary>
    /// Gets the project repository.
    /// </summary>
    IProjectRepository ProjectRepository { get; }

    /// <summary>
    /// Gets the task repository.
    /// </summary>
    ITaskRepository TaskRepository { get; }

    /// <summary>
    /// Gets the user repository.
    /// </summary>
    IUserRepository UserRepository { get; }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    Task Save();
}
