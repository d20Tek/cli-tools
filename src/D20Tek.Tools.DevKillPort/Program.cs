global using D20Tek.Functional;
global using D20Tek.Spectre.Console.Extensions;
global using Spectre.Console;
global using Spectre.Console.Cli;
global using D20Tek.Tools.Common;
global using D20Tek.Tools.Common.Controls;

using D20Tek.Tools.DevKillPort.Commands;

namespace D20Tek.Tools.DevKillPort;

internal class Program
{
    public static async Task<int> Main(string[] args)
    {
        return await new CommandAppBuilder().WithDIContainer()
                                            .WithStartup<Startup>()
                                            .WithDefaultCommand<InteractiveCommand>()
                                            .Build()
                                            .RunAsync(args);
    }
}
