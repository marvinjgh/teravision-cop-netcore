using System.Linq.Expressions;
using Backend.Service.Contracts;
using Backend.Service.Models;
using Backend.Service.Respository;
using Microsoft.EntityFrameworkCore;

namespace Backend.Service.Repository;

/// <summary>
/// Repository for managing <see cref="ProjectEntity"/> instances.
/// </summary>
/// <param name="repositoryContext">The database context.</param>
public class ProjectRepository(RepositoryContext repositoryContext) : RepositoryBase<ProjectEntity>(repositoryContext), IProjectRepository
{
    /// <summary>
    /// Gets a project by its identifier.
    /// </summary>
    /// <param name="id">The project identifier.</param>
    /// <param name="include">A flag indicating whether to include related entities (tasks).</param>
    /// <returns>A project that matches the specified identifier; otherwise, null.</returns>
    public Task<ProjectEntity?> GetProjectById(long id, bool include = false)
    {
        if (include)
        {
            return FindByCondition(project => project.Id == id)
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync();
        }
        else
        {
            return FindByCondition(project => project.Id == id).FirstOrDefaultAsync();
        }
    }
    /// <summary>
    /// Gets all projects, optionally filtered by a specific expression.
    /// </summary>
    /// <param name="expression">The expression to filter the projects.</param>
    /// <returns>A collection of projects.</returns>
    public async Task<IEnumerable<ProjectEntity>> GetAllProjects(Expression<Func<ProjectEntity, bool>>? expression)
    {
        if (expression is not null)
        {
            return await FindByCondition(expression).ToListAsync();
        }
        return await FindAll().ToListAsync();
    }
    /// <summary>
    /// Creates a new project.
    /// </summary>
    /// <param name="project">The project to create.</param>
    public void CreateProject(ProjectEntity project) => Create(project);
    /// <summary>
    /// Updates an existing project.
    /// </summary>
    /// <param name="project">The project to update.</param>
    public void UpdateProject(ProjectEntity project) => Update(project);
    /// <summary>
    /// Deletes a project.
    /// </summary>
    /// <param name="project">The project to delete.</param>
    public void DeleteProject(ProjectEntity project) => Delete(project);
}
