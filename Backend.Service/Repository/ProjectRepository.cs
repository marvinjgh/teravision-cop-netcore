using Backend.Service.Contracts;
using Backend.Service.Models;
using Backend.Service.Respository;
using Microsoft.EntityFrameworkCore;

namespace Backend.Service.Repository;

public class ProjectRepository : RepositoryBase<Project>, IProjectRepository
{
    public ProjectRepository(RepositoryContext repositoryContext) : base(repositoryContext)
    {
    }

    public Task<Project?> GetProjectById(long projectId)
    {
        return FindByCondition(project => project.Id == projectId).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Project>> GetAllProjects()
    {
        return await FindAll()
            .ToListAsync();
    }

    public void CreateProject(Project project) => Create(project);
    public void UpdateProject(Project project) => Update(project);
    public void DeleteProject(Project project) => Delete(project);

}
