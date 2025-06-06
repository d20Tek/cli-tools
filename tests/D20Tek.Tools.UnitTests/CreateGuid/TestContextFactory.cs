using D20Tek.Spectre.Console.Extensions;
using D20Tek.Spectre.Console.Extensions.Testing;
using D20Tek.Tools.CreateGuid.Contracts;
using D20Tek.Tools.UnitTests.CreateGuid.Fakes;
using Microsoft.Extensions.DependencyInjection;
using TextCopy;

namespace D20Tek.Tools.UnitTests.CreateGuid;

internal static class TestContextFactory
{
    public static CommandAppBuilderTestContext CreateWithGuid(Guid guid) =>
        CreateWithGuids([guid]);

    public static CommandAppBuilderTestContext CreateWithGuids(Guid[] guids)
    {
        var container = new ServiceCollection().AddSingleton<IGuidGenerator>(new FakeGuidGenerator(guids));

        return CreateWithContainer<FakeCreateGuidStartup>(container);
    }

    public static CommandAppBuilderTestContext CreateWithClipboard(Guid guid, FakeClipboard clipboard)
    {
        var container = new ServiceCollection().AddSingleton<IGuidGenerator>(new FakeGuidGenerator([guid]))
                                               .AddSingleton<IClipboard>(clipboard);

        return CreateWithContainer<FakeCreateGuidNoClipboardStartup>(container);
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
