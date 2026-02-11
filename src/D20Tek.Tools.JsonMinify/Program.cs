using D20Tek.Tools.JsonMinify.Commands;

namespace D20Tek.Tools.JsonMinify;

[ExcludeFromCodeCoverage]
public sealed class Program
{
    public static async Task<int> Main(string[] args) =>
        await new CommandAppBuilder().WithDIContainer()
                                     .WithStartup<Startup>()
                                     .WithDefaultCommand<MinifyFileCommand>()
                                     .Build()
                                     .RunAsync(args);
}