using D20Tek.NuGet.Portfolio.Features;

namespace D20Tek.NuGet.Portfolio.Configuration;

internal sealed class AppConfiguration : ICommandConfiguration
{
    public void Configure(IConfigurator config)
    {
        config.CaseSensitivity(CaseSensitivity.None)
              .SetApplicationName("nu-port")
              .SetApplicationVersion(Globals.AppVersion)
              .ValidateExamples();

        config.AddCommand<InteractiveCommand>("start")
              .WithDescription("Starts an interactive prompt for managing your NuGet Portfolio.")
              .WithExample(["start"]);
    }
}
