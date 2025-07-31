using Backend.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Service;

public class RepositoryContext : DbContext
{
    public RepositoryContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<Project> TodoItems { get; set; }
}

