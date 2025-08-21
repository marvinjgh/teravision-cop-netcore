using Backend.Service.Models;

namespace Backend.Service.Contracts;

public interface IProjectRepository
{
    Task<IEnumerable<Project>> GetAllProjects(bool include = false);
    Task<IEnumerable<Project>> GetAllActiveProjects(bool include = false);
    Task<Project?> GetProjectById(long projectId, bool include = false);
    void CreateProject(Project project);
    void UpdateProject(Project project);
    void DeleteProject(Project project);
}
