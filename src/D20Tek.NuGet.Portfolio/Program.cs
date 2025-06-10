using D20Tek.NuGet.Portfolio.Configuration;
using D20Tek.NuGet.Portfolio.Features;
using D20Tek.NuGet.Portfolio.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace D20Tek.NuGet.Portfolio;

public sealed class Program
{
    public static async Task<int> Main(string[] args) =>
        await new CommandAppBuilder().WithDIContainer(CreateAppServiceCollection())
                                     .WithStartup<Startup>()
                                     .WithDefaultCommand<InteractiveCommand>()
                                     .Build()
                                     .RunAsync(args);

    private static IServiceCollection CreateAppServiceCollection() =>
        new ServiceCollection().AddDatabase()
                               .AddLogging(config =>
                                {
                                    config.AddConsole();
                                    config.SetMinimumLevel(LogLevel.Information);
                                });
}
