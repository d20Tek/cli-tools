using D20Tek.Tools.DevPassword.Commands;
using D20Tek.Tools.DevPassword.Services;

namespace D20Tek.Tools.DevPassword;

internal sealed class Startup : StartupBase
{
    private const string _configFilename = Constants.ConfigFile;
    private const string _dataFolder = Constants.DataFolder;

    public override IConfigurator ConfigureCommands(IConfigurator config)
    {
        config.CaseSensitivity(CaseSensitivity.None);
        config.SetApplicationName(Constants.AppName);
        config.ValidateExamples();

        config.AddCommand<GeneratePasswordCommand>("generate")
              .WithAlias("gen")
              .WithDescription("Default command that a generates password based on defined settings.")
              .WithExample(["gen", "-c", "1"]);

        config.AddCommand<UpdateConfigCommand>("configure")
              .WithAlias("config")
              .WithAlias("c")
              .WithDescription("Configure various properties of the dev passwords.")
              .WithExample(["configure"]);

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
        registrar.WithLifetimes().RegisterSingleton<IPasswordGenerator, PasswordGenerator>();
    }
}
