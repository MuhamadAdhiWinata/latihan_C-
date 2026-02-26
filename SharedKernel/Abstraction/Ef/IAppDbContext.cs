using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SharedKernel.Abstraction.Ef;

public interface IAppDbContext
{
    
}

public static class AppDbContextExtensions
{
    public static void AddAppDbContext<T>(this IServiceCollection services, Action<DbContextOptionsBuilder> configure) where T : DbContext, IAppDbContext
    {
        services.AddDbContext<T>(configure);
        services.AddScoped<IAppDbContext, T>(x => x.GetRequiredService<T>());
    }   
}