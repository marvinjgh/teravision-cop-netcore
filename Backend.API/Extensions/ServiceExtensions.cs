using Microsoft.EntityFrameworkCore;
using Backend.Service;
using Backend.Service.Contracts;
using Backend.Service.Repository;

namespace Backend.API.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureDBContext(this IServiceCollection services)
    {
        services.AddDbContext<RepositoryContext>(opt => opt.UseInMemoryDatabase("FullStack"));
    }
    public static void ConfigureRepositoryWrapper(this IServiceCollection services)
    {
        services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
    }
}
