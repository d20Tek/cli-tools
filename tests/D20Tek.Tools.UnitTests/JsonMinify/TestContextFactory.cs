using D20Tek.Tools.JsonMinify;
using D20Tek.Tools.JsonMinify.Services;
using D20Tek.Tools.UnitTests.JsonMinify.Fakes;

namespace D20Tek.Tools.UnitTests.JsonMinify;

[ExcludeFromCodeCoverage]
internal static class TestContextFactory
{
    public static CommandAppBuilderTestContext Create() =>
        CreateWithContainer<Startup>(new ServiceCollection());

    public static CommandAppBuilderTestContext CreateWithFakeFileAdapter(IFileAdapter fileAdapter)
    {
        var container = new ServiceCollection();
        container.AddSingleton(fileAdapter);

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
