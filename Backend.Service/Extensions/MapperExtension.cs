using Backend.Service.DataTransferObjects;
using Backend.Service.Models;

namespace Backend.Service.Extensions;

/// <summary>
/// Provides extension methods for mapping entity models to data transfer objects (DTOs).
/// </summary>
public static class MapperExtension
{
    /// <summary>
    /// Converts a <see cref="ProjectEntity"/> to a <see cref="ProjectDTO"/>.
    /// </summary>
    /// <param name="entity">The project entity to convert.</param>
    /// <returns>A new <see cref="ProjectDTO"/> instance.</returns>
    public static ProjectDTO ToProjectDTO(this ProjectEntity entity) =>
        new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            IsDeleted = entity.IsDeleted,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };

    /// <summary>
    /// Converts a <see cref="TaskEntity"/> to a <see cref="TaskDTO"/>.
    /// </summary>
    /// <param name="entity">The task entity to convert.</param>
    /// <returns>A new <see cref="TaskDTO"/> instance.</returns>
    public static TaskDTO ToTaskDto(this TaskEntity entity) =>
        new()
        {
            Id = entity.Id,
            ProjectId = entity.ProjectId,
            Name = entity.Name,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
}
