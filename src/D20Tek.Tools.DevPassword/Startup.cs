using D20Tek.Spectre.Console.Extensions.Injection;
using D20Tek.Spectre.Console.Extensions.Services;
using D20Tek.Tools.DevPassword.Commands;
using D20Tek.Tools.DevPassword.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace D20Tek.Tools.DevPassword;

internal sealed class Startup : StartupBase
{
    private const string _configFilename = "config.json";
    private const string _dataFolder = "data";

    public override IConfigurator ConfigureCommands(IConfigurator config)
    {
        config.CaseSensitivity(CaseSensitivity.None);
        config.SetApplicationName("dev-password");
        config.ValidateExamples();

        config.AddCommand<GeneratePasswordCommand>("generate")
              .WithAlias("gen")
              .WithDescription("Default command that a generates password based on defined settings.")
              .WithExample(["gen", "-c", "1"]);

        return config;
    }

    public override void ConfigureServices(ITypeRegistrar registrar)
    {
        registrar.WithLifetimes().Services.AddLowDb<PasswordConfig>(b =>
            b.UseFileDatabase(_configFilename)
             .WithFolder(_dataFolder)
             .WithLifetime(ServiceLifetime.Singleton));

        registrar.WithConsoleVerbosityWriter();
        registrar.WithLifetimes().RegisterSingleton<IConfigurationService, ConfigurationService>();
    }
}
