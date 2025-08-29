using Backend.Service.DataTransferObjects;
using Backend.Service.Models;

namespace Backend.Service.Extensions;

public static class MapperExtension
{
    public static ProjectDTO ToProjectDTO(this Project entity) =>
        new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            IsDeleted = entity.IsDeleted,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };

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
