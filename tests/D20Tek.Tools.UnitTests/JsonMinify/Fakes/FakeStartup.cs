using D20Tek.Tools.JsonMinify.Commands;
using D20Tek.Tools.JsonMinify.Services;

namespace D20Tek.Tools.UnitTests.JsonMinify.Fakes;

internal sealed class FakeStartup : StartupBase
{
    public override IConfigurator ConfigureCommands(IConfigurator config)
    {
        config.CaseSensitivity(CaseSensitivity.None);
        config.SetApplicationName("json-minify-test");

        config.AddCommand<MinifyFileCommand>("file");
        config.AddCommand<MinifyFolderCommand>("folder");

        return config;

    }
    public override void ConfigureServices(ITypeRegistrar registrar) => 
        registrar.WithLifetimes().RegisterSingleton<IMinifyService, MinifyService>();
}
