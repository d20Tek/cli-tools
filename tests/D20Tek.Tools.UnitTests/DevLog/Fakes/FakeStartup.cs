using D20Tek.Tools.DevLog.Commands;

namespace D20Tek.Tools.UnitTests.DevLog.Fakes;

internal sealed class FakeStartup : StartupBase
{
    public override IConfigurator ConfigureCommands(IConfigurator config)
    {
        config.CaseSensitivity(CaseSensitivity.None);
        config.SetApplicationName("dev-log-test");

        config.AddCommand<AddEntryCommand>("add");
        config.AddCommand<EditEntryCommand>("edit");
        config.AddCommand<ViewLogCommand>("view");
        config.AddCommand<ListLogsCommand>("list");

        return config;
    }

    public override void ConfigureServices(ITypeRegistrar registrar) { }
}
