using D20Tek.Spectre.Console.Extensions.Testing;
using D20Tek.Tools.CreateGuid.Commands;
using D20Tek.Tools.UnitTests.CreateGuid.Fakes;

namespace D20Tek.Tools.UnitTests.CreateGuid;

internal static class TestContextFactory
{
    public static CommandAppTestContext CreateWithGuid(Guid guid) =>
        CreateWithGuids([guid]);

    public static CommandAppTestContext CreateWithGuids(Guid[] guids, FakeClipboard? clipboard = null)
    {
        var context = new CommandAppTestContext();
        context.Registrar.ConfigureServices(guids, clipboard);
        context.Configure(config =>
        {
            config.Settings.ApplicationName = "create-guid-test";
            config.AddCommand<CreateGuidCommand>("generate");
        });

        return context;
    }

    public static CommandAppTestContext CreateWithClipboard(Guid guid, FakeClipboard clipboard) =>
        CreateWithGuids([guid], clipboard);
}
