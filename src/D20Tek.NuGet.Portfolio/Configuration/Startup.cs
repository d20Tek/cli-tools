namespace D20Tek.NuGet.Portfolio.Configuration;

internal sealed class Startup : StartupBase
{
    public override IConfigurator ConfigureCommands(IConfigurator config) =>
        config.ApplyConfiguration(new AppConfiguration())
              .ApplyConfiguration(new CollectionCommandConfiguration())
              .ApplyConfiguration(new TrackedPackageCommandConfiguration());

    public override void ConfigureServices(ITypeRegistrar registrar)
    {
    }
}
