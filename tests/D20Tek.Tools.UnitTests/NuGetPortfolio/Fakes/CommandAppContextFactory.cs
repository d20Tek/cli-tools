using D20Tek.Functional;
using D20Tek.NuGet.Portfolio.Configuration;
using D20Tek.NuGet.Portfolio.Persistence;
using D20Tek.Spectre.Console.Extensions.Injection;
using D20Tek.Spectre.Console.Extensions.Testing;

namespace D20Tek.Tools.UnitTests.NuGetPortfolio.Fakes;

internal static class CommandAppContextFactory
{
    public static CommandAppTestContext CreateWithMemoryDb(AppDbContext? dbContext = null) =>
        new CommandAppTestContext().ToIdentity()
            .Iter(c => c.Configure(config => CommandsConfiguration.ConfigureCollections(config)))
            .Iter(c => c.Registrar.WithLifetimes()
                                  .RegisterSingleton<AppDbContext>(dbContext ?? InMemoryDbContext.Create()));
}
