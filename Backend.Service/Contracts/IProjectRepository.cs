using Backend.Service.Models;

namespace Backend.Service.Contracts;

public interface IProjectRepository
{
    // Task<IEnumerable<Project>> GetAllProjects();
    Task<Project?> GetProjectById(long projectId);
    Task<List<Project>> GetAllProjects();
    // void CreateProject(Project project);
    // void UpdateProject(Project project);
    // void DeleteProject(Project project);
}
