using D20Tek.Tools.DevPassword.Commands;
using D20Tek.Tools.DevPassword.Contracts;
using D20Tek.Tools.DevPassword.Services;

namespace D20Tek.Tools.UnitTests.DevPassword.Fakes;

internal sealed class FakeStartup : StartupBase
{
    public override IConfigurator ConfigureCommands(IConfigurator config)
    {
        config.Settings.ApplicationName = "dev-password-test";
        config.AddCommand<UpdateConfigCommand>("configure");
        return config;
    }

    public override void ConfigureServices(ITypeRegistrar registrar)
    {
        registrar.WithConsoleVerbosityWriter();
        registrar.WithLifetimes().RegisterSingleton<IConfigurationService, ConfigurationService>();
        registrar.WithLifetimes().RegisterSingleton<IPasswordGenerator, PasswordGenerator>();
    }
}
