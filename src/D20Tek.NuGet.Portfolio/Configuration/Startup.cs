namespace D20Tek.NuGet.Portfolio.Configuration;

internal sealed class Startup : StartupBase
{
    public override IConfigurator ConfigureCommands(IConfigurator config) =>
        config.ConfigureCommands();

    public override void ConfigureServices(ITypeRegistrar registrar)
    {
    }
}
