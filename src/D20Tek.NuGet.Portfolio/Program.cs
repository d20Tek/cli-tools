using D20Tek.NuGet.Portfolio.Configuration;
using D20Tek.NuGet.Portfolio.Features;

namespace D20Tek.NuGet.Portfolio;

public sealed class Program
{
    public static async Task<int> Main(string[] args)
    {
        var services = AppServiceCollection.Initialize();
        return await new CommandAppBuilder().WithDIContainer(services)
                                            .WithStartup<Startup>()
                                            .WithDefaultCommand<InteractiveCommand>()
                                            .Build()
                                            .RunAsync(args);
    }
}
