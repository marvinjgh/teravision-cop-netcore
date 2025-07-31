using Backend.Service.Contracts;

namespace Backend.Service.Repository;

public class RepositoryWrapper : IRepositoryWrapper
{
    private RepositoryContext _repoContext;
    private IProjectRepository _project;
    public IProjectRepository Project
    {
        get
        {
            _project ??= new ProjectRepository(_repoContext);
            return _project;
        }
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public RepositoryWrapper(RepositoryContext repositoryContext)
    {
        _repoContext = repositoryContext;
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public async Task Save()
    {
        await _repoContext.SaveChangesAsync();
    }
}
