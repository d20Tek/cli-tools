using D20Tek.Spectre.Console.Extensions.Services;
using D20Tek.Tools.DevKillPort.Commands;
using D20Tek.Tools.DevKillPort.Services;

namespace D20Tek.Tools.DevKillPort;

internal sealed class Startup : StartupBase
{
    public override IConfigurator ConfigureCommands(IConfigurator config)
    {
        config.CaseSensitivity(CaseSensitivity.None);
        config.SetApplicationName(Constants.AppName);
        config.ValidateExamples();

        config.AddCommand<KillPortCommand>("kill")
              .WithAlias("k")
              .WithDescription("Find and kill processes bound to a port.")
              .WithExample(["kill", "5000"])
              .WithExample(["kill", "5000", "--force"])
              .WithExample(["kill", "5000", "--dry-run"]);

        config.AddCommand<ViewPortCommand>("view")
              .WithAlias("v")
              .WithDescription("View processes bound to a port without killing them.")
              .WithExample(["view", "5000"])
              .WithExample(["view", "5000", "--json"]);

        return config;
    }

    public override void ConfigureServices(ITypeRegistrar registrar)
    {
        registrar.WithConsoleVerbosityWriter();

        var osAdapter = new OperatingSystemAdapter();
        var commandRunner = new CommandRunner();
        var resolver = PortResolverFactory.Create(osAdapter, commandRunner);

        registrar.RegisterInstance(typeof(ICommandRunner), commandRunner);
        registrar.RegisterInstance(typeof(IPortResolver), resolver);
        registrar.RegisterInstance(typeof(IProcessTerminator), new ProcessTerminator());
    }
}
