using Backend.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Service;

/// <summary>
/// Represents the database context for the application, providing access to the underlying database.
/// </summary>
public class RepositoryContext(DbContextOptions options) : DbContext(options)
{
    /// <summary>
    /// Configures the model that was discovered by convention from the entity types
    /// exposed in <see cref="DbSet{TEntity}"/> properties on your derived context.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProjectEntity>()
            .HasMany(p => p.Tasks)
            .WithOne(t => t.Project)
            .HasForeignKey(t => t.ProjectId);
    }


    /// <summary>
    /// Asynchronously saves all changes made in this context to the database.
    /// This method will automatically set the 'CreatedAt', 'UpdatedAt', and 'IsDeleted' properties on entities.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous save operation. The task result contains the
    /// number of state entries written to the database.
    /// </returns>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries();
        var now = DateTimeOffset.UtcNow;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added && entry.Metadata.FindProperty("CreatedAt") != null)
            {
                entry.Property("CreatedAt").CurrentValue = now;
            }
            if (entry.State == EntityState.Deleted && entry.Metadata.FindProperty("IsDeleted") != null && (entry.Property("IsDeleted").CurrentValue is bool isDeleted && isDeleted == false))
            {
                entry.Property("IsDeleted").CurrentValue = true;
                entry.State = EntityState.Modified;
            }

            entry.Property("UpdatedAt").CurrentValue = now;

        }
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Gets or sets the <see cref="DbSet{TEntity}"/> of projects.
    /// </summary>
    public DbSet<ProjectEntity> Projects { get; set; }
    /// <summary>
    /// Gets or sets the <see cref="DbSet{TEntity}"/> of tasks.
    /// </summary>
    public DbSet<TaskEntity> Tasks { get; set; }
}

