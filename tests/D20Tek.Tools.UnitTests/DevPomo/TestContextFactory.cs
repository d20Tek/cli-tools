using D20Tek.Tools.DevPomo;

namespace D20Tek.Tools.UnitTests.DevPomo;

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
