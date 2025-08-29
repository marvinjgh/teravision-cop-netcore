using System.Linq.Expressions;
using Backend.Service.Models;

namespace Backend.Service.Contracts;

public interface IProjectRepository
{
    Task<IEnumerable<ProjectEntity>> GetAllProjects(Expression<Func<ProjectEntity, bool>>? expression);
    Task<ProjectEntity?> GetProjectById(long projectId, bool include = false);
    void CreateProject(ProjectEntity project);
    void UpdateProject(ProjectEntity project);
    void DeleteProject(ProjectEntity project);
}
