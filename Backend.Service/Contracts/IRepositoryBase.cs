using System.Linq.Expressions;

namespace Backend.Service.Contracts;

/// <summary>
/// Represents the base repository with common CRUD operations.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
public interface IRepositoryBase<T>
{
    /// <summary>
    /// Finds all entities.
    /// </summary>
    /// <returns>An IQueryable of all entities.</returns>
    IQueryable<T> FindAll();

    /// <summary>
    /// Finds entities based on a condition.
    /// </summary>
    /// <param name="expression">The condition to filter entities.</param>
    /// <returns>An IQueryable of entities that match the condition.</returns>
    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);

    /// <summary>
    /// Creates a new entity.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    void Create(T entity);

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    void Update(T entity);

    /// <summary>
    /// Deletes an entity.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    void Delete(T entity);
}
