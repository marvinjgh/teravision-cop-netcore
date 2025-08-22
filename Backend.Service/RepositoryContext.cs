using Backend.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Service;

public class RepositoryContext : DbContext
{
    public RepositoryContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Project>()
            .HasMany(p => p.Tasks)
            .WithOne(t => t.Project)
            .HasForeignKey(t => t.ProjectId);
    }

    public override int SaveChanges()
    {
        var entries = ChangeTracker.Entries();
        var now = DateTimeOffset.UtcNow;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added && entry.Metadata.FindProperty("CreatedAt") != null)
            {
                entry.Property("CreatedAt").CurrentValue = now;
            }
            else if (entry.State == EntityState.Modified && entry.Metadata.FindProperty("UpdatedAt") != null)
            {
                entry.Property("UpdatedAt").CurrentValue = now;
            }
        }
        return base.SaveChanges();
    }

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

    public DbSet<Project> Projects { get; set; }
    public DbSet<TaskEntity> Tasks { get; set; }
}

