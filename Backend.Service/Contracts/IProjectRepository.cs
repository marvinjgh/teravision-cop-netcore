using Backend.Service.Models;

namespace Backend.Service.Contracts;

public interface IProjectRepository
{
    Task<IEnumerable<Project>> GetAllProjects();
    Task<IEnumerable<Project>> GetAllActiveProjects();
    Task<Project?> GetProjectById(long projectId);
    void CreateProject(Project project);
    void UpdateProject(Project project);
    void DeleteProject(Project project);
}
