using System.Linq.Expressions;
using Backend.Service.Contracts;
using Backend.Service.Models;
using Backend.Service.Respository;
using Microsoft.EntityFrameworkCore;

namespace Backend.Service.Repository;

/// <summary>
/// Repository for managing <see cref="UserEntity"/> instances.
/// </summary>
/// <param name="repositoryContext">The database context.</param>
public class UserRepository(RepositoryContext repositoryContext) : RepositoryBase<UserEntity>(repositoryContext), IUserRepository
{
    /// <summary>
    /// Gets a user by its identifier.
    /// </summary>
    /// <param name="id">The user identifier.</param>
    /// <param name="include">A flag indicating whether to include related entities.</param>
    /// <returns>A user that matches the specified identifier; otherwise, null.</returns>
    public Task<UserEntity?> GetUserById(long id)
    {
        return FindByCondition(task => task.Id == id).FirstOrDefaultAsync();
    }
    /// <summary>
    /// Gets all users, optionally filtered by a specific expression.
    /// </summary>
    /// <param name="expression">The expression to filter the users.</param>
    /// <returns>A collection of users.</returns>
    public async Task<IEnumerable<UserEntity>> GetAllUsers(Expression<Func<UserEntity, bool>>? expression)
    {
        if (expression is not null)
        {
            return await FindByCondition(expression).ToListAsync();
        }
        return await FindAll().ToListAsync();
    }
    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="user">The user to create.</param>
    public void CreateUser(UserEntity user) => Create(user);
    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="user">The user to update.</param>
    public void UpdateUser(UserEntity user) => Update(user);
    /// <summary>
    /// Deletes a user.
    /// </summary>
    /// <param name="user">The user to delete.</param>
    public void DeleteUser(UserEntity user) => Delete(user);
}
