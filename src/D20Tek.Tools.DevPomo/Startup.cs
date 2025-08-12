using D20Tek.Spectre.Console.Extensions;
using D20Tek.Spectre.Console.Extensions.Injection;
using D20Tek.Spectre.Console.Extensions.Services;
using D20Tek.Tools.DevPomo.Commands.RunTimer;
using Spectre.Console.Cli;

namespace D20Tek.Tools.DevPomo;

internal sealed class Startup : StartupBase
{
    public override void ConfigureServices(ITypeRegistrar registrar)
    {
        registrar.WithConsoleVerbosityWriter();
        //registrar.WithLifetimes().RegisterSingleton<IGuidGenerator, GuidGenerator>()
        //                         .RegisterSingleton<IGuidFormatter, GuidFormatter>()
        //                         .RegisterSingleton<IClipboard, Clipboard>();
    }

    public override IConfigurator ConfigureCommands(IConfigurator config)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        config.CaseSensitivity(CaseSensitivity.None);
        config.SetApplicationName("dev-pomo");
        config.ValidateExamples();

        config.AddCommand<RunTimerCommand>("run-timer")
              .WithAlias("run")
              .WithAlias("r")
              .WithDescription("Default command that runs the pomodoro timer.")
              .WithExample(["run-timer"]);

        return config;
    }
}
