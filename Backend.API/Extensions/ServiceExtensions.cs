using Microsoft.EntityFrameworkCore;
using Backend.Service;
using Backend.Service.Contracts;
using Backend.Service.Repository;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;

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
                    new OpenApiServer{ Url= "https://localhost:8081/"},
                    new OpenApiServer{ Url= "http://localhost:8080/"},
                ];
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "JWT Authentication",
                    Description = "Enter JWT Bearer token **_only_**",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer", // must be lower case
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes.Add(JwtBearerDefaults.AuthenticationScheme, securityScheme);
                document.SecurityRequirements.Add(new OpenApiSecurityRequirement
                {
                    { securityScheme, [] }
                });
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
            options.AddOperationTransformer((operation, context, asdf) =>
            {
                if (context.Description.ActionDescriptor.EndpointMetadata.Any(m => m is AllowAnonymousAttribute))
                {
                    operation.Security = [
                        new OpenApiSecurityRequirement
                        {
                            { new OpenApiSecurityScheme(), new List<string>() }
                        }
                    ];
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

    public static void ConfigureAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(opt =>
        {
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = configuration["AppSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = configuration["AppSettings:Audience"],
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AppSettings:Token"]!)),
                ValidateIssuerSigningKey = true,
            };
        });
    }
}
