using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace D20Tek.NuGet.Portfolio.Persistence;

internal static class DependencyInjection
{
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddMemoryCache();

        // Register the DbContext with SQLite
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite("Data Source=nu-port.db"));

        return services;
    }
}
