using Backend.Service.Contracts;
using Backend.Service.Models;
using Backend.Service.Respository;
using Microsoft.EntityFrameworkCore;

namespace Backend.Service.Repository;

public class ProjectRepository(RepositoryContext repositoryContext) : RepositoryBase<Project>(repositoryContext), IProjectRepository
{
    public Task<Project?> GetProjectById(long id, bool include = false)
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
    public async Task<IEnumerable<Project>> GetAllProjects(bool include = false)
    {
        if (include)
        {
            return await FindAll()
                .Include(p => p.Tasks)
                .ToListAsync();
        }
        else
        {
            return await FindAll().ToListAsync();
        }
    }
    public async Task<IEnumerable<Project>> GetAllActiveProjects(bool include = false)
    {
        if (include)
        {
            return await FindByCondition(project => !project.IsDeleted)
                .Include(p => p.Tasks)
                .ToListAsync();
        }
        else
        {
            return await FindByCondition(project => !project.IsDeleted).ToListAsync();
        }
    }
    public void CreateProject(Project project) => Create(project);
    public void UpdateProject(Project project) => Update(project);
    public void DeleteProject(Project project) => Delete(project);
}
