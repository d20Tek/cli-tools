using D20Tek.Tools.DevPomo.Commands.RunTimer;

namespace D20Tek.Tools.DevPomo;

[ExcludeFromCodeCoverage]
public sealed class Program
{
    public static async Task<int> Main(string[] args) =>
        await new CommandAppBuilder().WithDIContainer()
                                     .WithStartup<Startup>()
                                     .WithDefaultCommand<RunTimerCommand>()
                                     .Build()
                                     .RunAsync(args);
}