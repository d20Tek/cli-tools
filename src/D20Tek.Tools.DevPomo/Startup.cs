using D20Tek.Spectre.Console.Extensions.Injection;
using D20Tek.Tools.DevPomo.Commands.Configuration;
using D20Tek.Tools.DevPomo.Commands.RunTimer;
using D20Tek.Tools.DevPomo.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace D20Tek.Tools.DevPomo;

internal sealed class Startup : StartupBase
{
    private const string _configFilename = "config.json";
    private const string _dataFolder = "data";

    public override void ConfigureServices(ITypeRegistrar registrar)
    {
        registrar.WithLifetimes().Services.AddLowDb<TimerConfiguration>(b =>
            b.UseFileDatabase(_configFilename)
             .WithFolder(_dataFolder)
             .WithLifetime(ServiceLifetime.Singleton));

        registrar.WithLifetimes().RegisterSingleton<IConfigurationService, ConfigurationService>();
    }

    public override IConfigurator ConfigureCommands(IConfigurator config)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        config.CaseSensitivity(CaseSensitivity.None);
        config.SetApplicationName(Constants.AppTitle);
        config.ValidateExamples();

        config.AddCommand<RunTimerCommand>("run-timer")
              .WithAlias("run")
              .WithAlias("r")
              .WithDescription("Default command that runs the pomodoro timer.")
              .WithExample(["run-timer"]);

        config.AddCommand<UpdateConfigCommand>("configure")
              .WithAlias("config")
              .WithAlias("c")
              .WithDescription("Configure various properties of the pomodoro timer.")
              .WithExample(["configure"]);

        return config;
    }
}
