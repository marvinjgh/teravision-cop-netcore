using System.Linq.Expressions;
using Backend.Service.Models;

namespace Backend.Service.Contracts;

/// <summary>
/// Represents the repository for managing projects.
/// </summary>
public interface IProjectRepository
{
    /// <summary>
    /// Gets all projects based on an optional expression.
    /// </summary>
    /// <param name="expression">The expression to filter projects.</param>
    /// <returns>A collection of projects.</returns>
    Task<IEnumerable<ProjectEntity>> GetAllProjects(Expression<Func<ProjectEntity, bool>>? expression);

    /// <summary>
    /// Gets a project by its ID.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="include">A flag to indicate whether to include related entities.</param>
    /// <returns>The project entity if found; otherwise, null.</returns>
    Task<ProjectEntity?> GetProjectById(long projectId, bool include = false);

    /// <summary>
    /// Creates a new project.
    /// </summary>
    /// <param name="project">The project to create.</param>
    void CreateProject(ProjectEntity project);

    /// <summary>
    /// Updates an existing project.
    /// </summary>
    /// <param name="project">The project to update.</param>
    void UpdateProject(ProjectEntity project);

    /// <summary>
    /// Deletes a project.
    /// </summary>
    /// <param name="project">The project to delete.</param>
    void DeleteProject(ProjectEntity project);
}
