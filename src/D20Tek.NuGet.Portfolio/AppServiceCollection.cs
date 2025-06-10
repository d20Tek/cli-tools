using D20Tek.NuGet.Portfolio.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace D20Tek.NuGet.Portfolio;

internal static class AppServiceCollection
{
    public static IServiceCollection Initialize() =>
        CreateAppServiceCollection().ApplyMigrations();

    private static IServiceCollection CreateAppServiceCollection() =>
        new ServiceCollection().AddDatabase()
                               .AddLogging(config =>
                               {
                                   config.AddConsole();
                                   config.SetMinimumLevel(LogLevel.Information);
                               });

    private static IServiceCollection ApplyMigrations(this IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.ApplyMigrations();

        return services;
    }
}
