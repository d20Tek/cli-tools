using D20Tek.NuGet.Portfolio.Features;

namespace D20Tek.NuGet.Portfolio.Configuration;

internal static class CommandsConfiguration
{
    public static IConfigurator ConfigureCommands(this IConfigurator config)
    {
        config.CaseSensitivity(CaseSensitivity.None)
              .SetApplicationName("nu-port")
              .SetApplicationVersion(Globals.AppVersion)
              .ValidateExamples();

        config.AddCommand<InteractiveCommand>("start")
              .WithDescription("Starts an interactive prompt for managing your NuGet Portfolio.")
              .WithExample(["start"]);

        return config;
    }
}
