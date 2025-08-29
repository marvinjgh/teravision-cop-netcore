using System.Linq.Expressions;
using Backend.Service.Models;

namespace Backend.Service.Contracts;

public interface IProjectRepository
{
    Task<IEnumerable<Project>> GetAllProjects(Expression<Func<Project, bool>>? expression);
    Task<Project?> GetProjectById(long projectId, bool include = false);
    void CreateProject(Project project);
    void UpdateProject(Project project);
    void DeleteProject(Project project);
}
