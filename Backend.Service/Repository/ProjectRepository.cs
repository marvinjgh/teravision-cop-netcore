using System.Linq.Expressions;
using Backend.Service.Contracts;
using Backend.Service.Models;
using Backend.Service.Respository;
using Microsoft.EntityFrameworkCore;

namespace Backend.Service.Repository;

public class ProjectRepository(RepositoryContext repositoryContext) : RepositoryBase<ProjectEntity>(repositoryContext), IProjectRepository
{
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
    public async Task<IEnumerable<ProjectEntity>> GetAllProjects(Expression<Func<ProjectEntity, bool>>? expression)
    {
        if (expression is not null)
        {
            return await FindByCondition(expression).ToListAsync();
        }
        return await FindAll().ToListAsync();
    }
    public void CreateProject(ProjectEntity project) => Create(project);
    public void UpdateProject(ProjectEntity project) => Update(project);
    public void DeleteProject(ProjectEntity project) => Delete(project);
}
