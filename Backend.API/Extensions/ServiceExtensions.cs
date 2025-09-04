using Microsoft.EntityFrameworkCore;
using Backend.Service;
using Backend.Service.Contracts;
using Backend.Service.Repository;
using Microsoft.OpenApi.Models;

namespace Backend.API.Extensions;

/// <summary>
/// Provides extension methods for service configuration.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Configures the database context for the application.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="config">The <see cref="IConfiguration"/> for the application.</param>
    public static void ConfigureDBContext(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<RepositoryContext>(
                opt => opt
                    .UseSqlServer(
                        config["ConnectionStrings:DefaultConnection"],
                        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()
                    )
            );
    }

    /// <summary>
    /// Configures the repository wrapper for dependency injection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    public static void ConfigureRepositoryWrapper(this IServiceCollection services)
    {
        services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
    }

    /// <summary>
    /// Configures OpenAPI for the application.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    public static void ConfigureOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi("copapi", options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info = new()
                {
                    Title = "CoP - Backend",
                    Version = "0.0.1",
                    Description = "An API for CoP."
                };
                document.Servers = [
                    new OpenApiServer{ Url= "http://localhost:8080/"},
                    new OpenApiServer{ Url= "https://localhost:8081/"},
                ];
                return Task.CompletedTask;
            });
            options.AddSchemaTransformer((schema, context, cancellationToken) =>
            {
                if (context.JsonTypeInfo.Type.FullName?.EndsWith("DTO") == true)
                {
                    schema.Title = context.JsonTypeInfo.Type.Name.Replace("DTO", "");
                }
                if (context.JsonTypeInfo.Type == typeof(long))
                {
                    schema.Format = "long";
                }

                return Task.CompletedTask;
            });
        });
    }

    /// <summary>
    /// Applies any pending database migrations.
    /// </summary>
    /// <param name="services">The <see cref="IServiceProvider"/> to get services from.</param>
    public static void ApplyMigrations(this IServiceProvider services)
    {
        var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<RepositoryContext>();
        var pendingMigrations = dbContext.Database.GetPendingMigrations();
        if (pendingMigrations.Any())
        {
            try
            {
                dbContext.Database.Migrate();
            }
            catch (Exception exception)
            {
                // TODO Add a logger
                Console.WriteLine(exception.Message);
            }
        }
    }
}
