using D20Tek.Spectre.Console.Extensions;
using D20Tek.Spectre.Console.Extensions.Services;
using D20Tek.Tools.CreateGuid.Commands;
using D20Tek.Tools.CreateGuid.Services;
using Spectre.Console.Cli;
using TextCopy;

namespace D20Tek.Tools.UnitTests.CreateGuid.Fakes;

internal class FakeCreateGuidStartup : StartupBase
{

    public override IConfigurator ConfigureCommands(IConfigurator config)
    {
        config.Settings.ApplicationName = "create-guid-test";
        config.AddCommand<CreateGuidCommand>("generate");
        return config;
    }

    public override void ConfigureServices(ITypeRegistrar registrar)
    {
        registrar.Register(typeof(IClipboard), typeof(FakeClipboard));
        registrar.Register(typeof(IGuidFormatter), typeof(GuidFormatter));
        registrar.Register(typeof(IVerbosityWriter), typeof(ConsoleVerbosityWriter));
    }
}

internal sealed class FakeCreateGuidNoClipboardStartup : FakeCreateGuidStartup
{
    public override void ConfigureServices(ITypeRegistrar registrar)
    {
        registrar.Register(typeof(IGuidFormatter), typeof(GuidFormatter));
        registrar.Register(typeof(IVerbosityWriter), typeof(ConsoleVerbosityWriter));
    }
}
