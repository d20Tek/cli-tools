using D20Tek.Tools.DevPassword;
using D20Tek.Tools.DevPassword.Contracts;
using D20Tek.Tools.UnitTests.DevPassword.Fakes;

namespace D20Tek.Tools.UnitTests.DevPassword;

[ExcludeFromCodeCoverage]
internal static class TestContextFactory
{
    public static CommandAppBuilderTestContext Create() =>
        CreateWithContainer<Startup>(new ServiceCollection());

    public static CommandAppBuilderTestContext CreateWithMemoryLowDb()
    {
        var container = new ServiceCollection();
        container.AddLowDb<PasswordConfig>(b =>
            b.UseInMemoryDatabase()
             .WithLifetime(ServiceLifetime.Singleton));

        return CreateWithContainer<FakeStartup>(container);
    }

    private static CommandAppBuilderTestContext CreateWithContainer<TStartup>(IServiceCollection container)
        where TStartup : StartupBase, new()
    {
        var context = new CommandAppBuilderTestContext();
        context.Builder.WithDIContainer(container)
                       .WithStartup<TStartup>();

        return context;
    }
}
