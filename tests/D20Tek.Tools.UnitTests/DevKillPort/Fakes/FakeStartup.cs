using D20Tek.Tools.DevKillPort.Commands;

namespace D20Tek.Tools.UnitTests.DevKillPort.Fakes;

internal sealed class FakeStartup : StartupBase
{
    public override IConfigurator ConfigureCommands(IConfigurator config)
    {
        config.CaseSensitivity(CaseSensitivity.None);
        config.SetApplicationName("dev-killport-test");

        config.AddCommand<KillPortCommand>("kill");
        config.AddCommand<ListPortCommand>("list");

        return config;
    }

    public override void ConfigureServices(ITypeRegistrar registrar) { }
}
