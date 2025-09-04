using Backend.Service.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Backend.Service.Respository;

/// <summary>
/// An abstract base class for repositories.
/// </summary>
/// <typeparam name="T">The type of entity.</typeparam>
public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    /// <summary>
    /// The database context.
    /// </summary>
    protected RepositoryContext RepositoryContext { get; set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryBase{T}"/> class.
    /// </summary>
    /// <param name="repositoryContext">The database context.</param>
    public RepositoryBase(RepositoryContext repositoryContext)
    {
        RepositoryContext = repositoryContext;
    }
    /// <summary>
    /// Finds all entities.
    /// </summary>
    /// <returns>A queryable collection of all entities.</returns>
    public IQueryable<T> FindAll() => RepositoryContext.Set<T>().AsNoTracking();
    /// <summary>
    /// Finds entities by condition.
    /// </summary>
    /// <param name="expression">The expression to filter entities.</param>
    /// <returns>A queryable collection of entities that match the condition.</returns>
    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression) =>
        RepositoryContext.Set<T>().Where(expression).AsNoTracking();
    /// <summary>
    /// Creates a new entity.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    public void Create(T entity) => RepositoryContext.Set<T>().Add(entity);
    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    public void Update(T entity) => RepositoryContext.Set<T>().Update(entity);
    /// <summary>
    /// Deletes an entity.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    public void Delete(T entity) => RepositoryContext.Set<T>().Remove(entity);
}