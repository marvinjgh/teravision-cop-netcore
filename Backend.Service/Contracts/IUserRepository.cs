using System.Linq.Expressions;
using Backend.Service.Models;

namespace Backend.Service.Contracts;

/// <summary>
/// Represents the repository for managing users.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Gets all users based on an optional expression.
    /// </summary>
    /// <param name="expression">The expression to filter users.</param>
    /// <returns>A collection of users.</returns>
    Task<IEnumerable<UserEntity>> GetAllUsers(Expression<Func<UserEntity, bool>>? expression);

    /// <summary>
    /// Gets a user by its ID.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="include">A flag to indicate whether to include related entities.</param>
    /// <returns>The user entity if found; otherwise, null.</returns>
    Task<UserEntity?> GetUserById(long userId);

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="user">The user to create.</param>
    void CreateUser(UserEntity user);

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="user">The user to update.</param>
    void UpdateUser(UserEntity user);

    /// <summary>
    /// Deletes a user.
    /// </summary>
    /// <param name="user">The user to delete.</param>
    void DeleteUser(UserEntity user);
}
