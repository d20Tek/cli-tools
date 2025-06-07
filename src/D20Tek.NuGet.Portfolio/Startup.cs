using D20Tek.Spectre.Console.Extensions;
using Spectre.Console.Cli;

namespace D20Tek.NuGet.Portfolio;

internal sealed class Startup : StartupBase
{
    public override IConfigurator ConfigureCommands(IConfigurator config)
    {
        return config;
    }

    public override void ConfigureServices(ITypeRegistrar registrar)
    {
    }
}
