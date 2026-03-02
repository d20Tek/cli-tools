using D20Tek.Spectre.Console.Extensions.Injection;
using D20Tek.Spectre.Console.Extensions.Services;
using D20Tek.Tools.DevLog.Commands;
using D20Tek.Tools.DevLog.Services;

namespace D20Tek.Tools.DevLog;

internal sealed class Startup : StartupBase
{
    public override IConfigurator ConfigureCommands(IConfigurator config)
    {
        config.CaseSensitivity(CaseSensitivity.None);
        config.SetApplicationName(Constants.AppName);
        config.ValidateExamples();

        config.AddCommand<AddEntryCommand>("add")
              .WithDescription("Add a project entry to this week's dev-log.")
              .WithExample(["add", "MyProject", "-f", "."]);

        return config;
    }

    public override void ConfigureServices(ITypeRegistrar registrar)
    {
        registrar.WithConsoleVerbosityWriter();

        registrar.WithLifetimes().RegisterSingleton<IFileSystemAdapter, FileSystemAdapter>();
        registrar.WithLifetimes().RegisterSingleton<IDevLogService, DevLogService>();
    }
}
