namespace D20Tek.NuGet.Portfolio.Common;

public static class ConfiguratorExtensions
{
    public static IConfigurator ApplyConfiguration(this IConfigurator configurator, ICommandConfiguration config)
    {
        config.Configure(configurator);
        return configurator;
    }
}
