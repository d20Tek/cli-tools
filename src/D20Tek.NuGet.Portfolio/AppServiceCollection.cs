using D20Tek.NuGet.Portfolio.Abstractions;
using D20Tek.NuGet.Portfolio.Persistence;
using D20Tek.NuGet.Portfolio.Services;
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
                               })
                               .AddServiceClients();

    private static IServiceCollection ApplyMigrations(this IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.ApplyMigrations();

        return services;
    }

    private static IServiceCollection AddServiceClients(this IServiceCollection services) =>
        services.ToIdentity()
                .Iter(s => s.AddHttpClient<INuGetSearchClient, NuGetScrapingClient>(client =>
                    {
                        client.BaseAddress = new Uri("https://www.nuget.org/");
                        client.DefaultRequestHeaders.UserAgent.ParseAdd("NuGet.Portfolio/1.0");
                    }))
                .Get();
}
