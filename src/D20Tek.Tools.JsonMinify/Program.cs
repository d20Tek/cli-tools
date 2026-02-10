using D20Tek.Tools.JsonMinify.Commands;
using D20Tek.Tools.JsonMinify;

namespace D20Tek.Tools.DevPassword;

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