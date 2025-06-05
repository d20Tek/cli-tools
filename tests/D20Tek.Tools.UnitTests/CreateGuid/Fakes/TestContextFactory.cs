using D20Tek.Spectre.Console.Extensions.Testing;
using D20Tek.Tools.CreateGuid.Commands;

namespace D20Tek.Tools.UnitTests.CreateGuid.Fakes;

internal static class TestContextFactory
{
    public static CommandAppTestContext CreateWithGuid(Guid guid) =>
        CreateWithGuids([guid]);

    public static CommandAppTestContext CreateWithGuids(Guid[] guid)
    {
        var context = new CommandAppTestContext();
        context.Registrar.ConfigureServices(guid);
        context.Configure(config =>
        {
            config.Settings.ApplicationName = "create-guid-test";
            config.AddCommand<CreateGuidCommand>("generate");
        });

        return context;
    }
}
