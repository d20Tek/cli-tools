using D20Tek.Tools.DevPassword;

namespace D20Tek.Tools.UnitTests.DevPassword;

[ExcludeFromCodeCoverage]
internal static class TestContextFactory
{
    public static CommandAppBuilderTestContext Create() =>
        CreateWithContainer<Startup>(new ServiceCollection());

    private static CommandAppBuilderTestContext CreateWithContainer<TStartup>(IServiceCollection container)
        where TStartup : StartupBase, new()
    {
        var context = new CommandAppBuilderTestContext();
        context.Builder.WithDIContainer(container)
                       .WithStartup<TStartup>();

        return context;
    }
}
