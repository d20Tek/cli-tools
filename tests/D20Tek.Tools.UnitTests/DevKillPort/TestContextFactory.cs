using D20Tek.Tools.DevKillPort;
using D20Tek.Tools.DevKillPort.Services;
using D20Tek.Tools.UnitTests.DevKillPort.Fakes;

namespace D20Tek.Tools.UnitTests.DevKillPort;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal static class TestContextFactory
{
    public static CommandAppBuilderTestContext Create() =>
        CreateWithContainer<Startup>(new ServiceCollection());

    public static CommandAppBuilderTestContext CreateWithFakes(
        IPortResolver resolver,
        IProcessTerminator terminator)
    {
        var container = new ServiceCollection();
        container.AddSingleton(resolver);
        container.AddSingleton(terminator);
        container.AddSingleton<ICommandRunner, FakeCommandRunner>();

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
