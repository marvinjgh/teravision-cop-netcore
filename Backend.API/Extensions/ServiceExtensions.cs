using Microsoft.EntityFrameworkCore;
using Backend.Service;
using Backend.Service.Contracts;
using Backend.Service.Repository;
using Microsoft.OpenApi.Models;

namespace Backend.API.Extensions;

public static class ServiceExtensions
{
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
    public static void ConfigureRepositoryWrapper(this IServiceCollection services)
    {
        services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
    }
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
                    new OpenApiServer{ Url= "https://localhost:8081/"},
                    new OpenApiServer{ Url= "http://localhost:8081/"}
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

    public static void ApplyMigrations(this IServiceProvider services)
    {
        var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<RepositoryContext>();
        if (dbContext.Database.HasPendingModelChanges())
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
