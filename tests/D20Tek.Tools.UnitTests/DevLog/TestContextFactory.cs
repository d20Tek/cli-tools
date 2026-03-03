using D20Tek.Tools.Common.Services;
using D20Tek.Tools.DevLog;
using D20Tek.Tools.DevLog.Services;
using D20Tek.Tools.UnitTests.DevLog.Fakes;

namespace D20Tek.Tools.UnitTests.DevLog;

[ExcludeFromCodeCoverage]
internal static class TestContextFactory
{
    public static CommandAppBuilderTestContext Create() =>
        CreateWithContainer<Startup>(new ServiceCollection());

    public static CommandAppBuilderTestContext CreateWithFakeFileAdapter(IFileSystemAdapter fileAdapter)
    {
        var container = new ServiceCollection();
        container.AddSingleton(fileAdapter);
        container.AddSingleton<IDevLogService, DevLogService>();

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
