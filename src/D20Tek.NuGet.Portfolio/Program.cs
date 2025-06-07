using D20Tek.Spectre.Console.Extensions;

namespace D20Tek.NuGet.Portfolio;

public sealed class Program
{
    public static async Task<int> Main(string[] args) =>
        await new CommandAppBuilder().WithDIContainer()
                                     .WithStartup<Startup>()
                                     //.WithDefaultCommand<CreateGuidCommand>()
                                     .Build()
                                     .RunAsync(args);
}
