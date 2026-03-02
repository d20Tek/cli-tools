using D20Tek.Spectre.Console.Extensions.Services;

namespace D20Tek.Tools.DevLog;

internal sealed class Startup : StartupBase
{
    public override IConfigurator ConfigureCommands(IConfigurator config)
    {
        config.CaseSensitivity(CaseSensitivity.None);
        config.SetApplicationName(Constants.AppName);
        config.ValidateExamples();

        // todo: add command configuration here.
        return config;
    }

    public override void ConfigureServices(ITypeRegistrar registrar)
    {
        registrar.WithConsoleVerbosityWriter();

        // todo: register services here.
    }
}
