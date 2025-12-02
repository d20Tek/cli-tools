using D20Tek.NuGet.Portfolio.Configuration;
using D20Tek.NuGet.Portfolio.Persistence;

namespace D20Tek.Tools.UnitTests.NuGetPortfolio.Fakes;

internal static class CommandAppContextFactory
{
    public static CommandAppTestContext CreateWithMemoryDb(AppDbContext? dbContext = null) =>
        new CommandAppTestContext().ToIdentity()
            .Iter(c => c.Configure(ConfigureCommands))
            .Iter(c => c.Registrar.WithLifetimes()
                                  .RegisterSingleton<AppDbContext>(dbContext ?? InMemoryDbContext.Create()));

    private static void ConfigureCommands(this IConfigurator config) =>
        config.ApplyConfiguration(new AppConfiguration())
              .ApplyConfiguration(new CollectionCommandConfiguration())
              .ApplyConfiguration(new TrackedPackageCommandConfiguration())
              .ApplyConfiguration(new PackageDownloadsConfiguration())
              .ApplyConfiguration(new SnapshotsCommandConfiguration());
}
