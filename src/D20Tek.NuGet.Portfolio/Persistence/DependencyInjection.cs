using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace D20Tek.NuGet.Portfolio.Persistence;

internal static class DependencyInjection
{
    public static IServiceCollection AddDatabase(this IServiceCollection services) =>
        services.ToIdentity()
                .Iter(s => s.AddMemoryCache())
                .Iter(s => s.AddDbContext<AppDbContext>(options =>
                    options.UseSqlite("Data Source=nu-port.db")))
                .Get();
}
