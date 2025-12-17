namespace D20Tek.Tools.DevPassword;

internal sealed class Startup : StartupBase
{
    public override IConfigurator ConfigureCommands(IConfigurator config)
    {
        config.CaseSensitivity(CaseSensitivity.None);
        config.SetApplicationName("dev-password");
        config.ValidateExamples();

        //config.AddCommand<CreateGuidCommand>("generate")
        //      .WithAlias("gen")
        //      .WithDescription("Default command that generates GUIDs in the appropriate format.")
        //      .WithExample(["gen", "-c", "1", "-f", "default", "-v", "normal"]);

        return config;
    }

    public override void ConfigureServices(ITypeRegistrar registrar)
    {
    }
}
