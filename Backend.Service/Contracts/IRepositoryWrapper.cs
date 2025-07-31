namespace Backend.Service.Contracts;

public interface IRepositoryWrapper
{
    IProjectRepository Project { get; }
    Task Save();
}
