using D20Tek.Tools.JsonMinify.Commands;
using D20Tek.Tools.JsonMinify.Services;

namespace D20Tek.Tools.JsonMinify;

internal sealed class Startup : StartupBase
{
    public override IConfigurator ConfigureCommands(IConfigurator config)
    {
        config.CaseSensitivity(CaseSensitivity.None);
        config.SetApplicationName(Constants.AppName);
        config.ValidateExamples();

        config.AddCommand<MinifyFileCommand>("file")
              .WithDescription("Default command that minifies the specified json file.")
              .WithExample(["file", ".\\test.json"]);

        return config;
    }

    public override void ConfigureServices(ITypeRegistrar registrar)
    {
        registrar.WithLifetimes().RegisterSingleton<IFileAdapter, FileAdapter>();
        registrar.WithLifetimes().RegisterSingleton<IMinifyService, MinifyService>();
    }
}
