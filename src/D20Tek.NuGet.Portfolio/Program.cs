using D20Tek.NuGet.Portfolio.Features;

namespace D20Tek.NuGet.Portfolio;

public sealed class Program
{
    public static async Task<int> Main(string[] args) =>
        await new CommandAppBuilder().WithDIContainer()
                                     .WithStartup<Startup>()
                                     .WithDefaultCommand<InteractiveCommand>()
                                     .Build()
                                     .RunAsync(args);
}
