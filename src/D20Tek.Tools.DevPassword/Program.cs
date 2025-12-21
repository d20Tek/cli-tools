using D20Tek.Tools.DevPassword.Commands;

namespace D20Tek.Tools.DevPassword;

[ExcludeFromCodeCoverage]
public sealed class Program
{
    public static async Task<int> Main(string[] args) =>
        await new CommandAppBuilder().WithDIContainer()
                                     .WithStartup<Startup>()
                                     .WithDefaultCommand<GeneratePasswordCommand>()
                                     .Build()
                                     .RunAsync(args);
}