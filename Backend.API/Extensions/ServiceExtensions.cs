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
        var connectionString = config["dbserverconnection:connectionString"];

        services.AddDbContext<RepositoryContext>(opt => opt.UseSqlite(connectionString));
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
                    new OpenApiServer{ Url= "https://localhost:7000/"},
                    new OpenApiServer{ Url= "http://localhost:5200/"}
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
}
