using D20Tek.Tools.CreateGuid;
using Spectre.Console;

namespace D20Tek.Tools.UnitTests.CreateGuid;

[TestClass]
public class End2EndTests
{
    [TestMethod]
    [TestCategory("E2E")]
    public async Task Run_WithDefaultArgs()
    {
        // arrange
        var args = Array.Empty<string>();
        AnsiConsole.Record();

        // act
        var result = await Program.Main(args);

        // assert
        Assert.AreEqual(0, result);
        await Task.Delay(200);
        var output = AnsiConsole.ExportText();
        StringAssert.StartsWith(output, "create-guid: running");
        StringAssert.Contains(output, "Command completed successfully!");
    }

    [TestMethod]
    [TestCategory("E2E")]
    public async Task Run_WithCountArgs()
    {
        // arrange
        var args = new string[] { "--count", "10" };
        AnsiConsole.Record();

        // act
        var result = await Program.Main(args);

        // assert
        Assert.AreEqual(0, result);
        await Task.Delay(200);
        var output = AnsiConsole.ExportText();
        StringAssert.Contains(output, "create-guid: running");
        StringAssert.Contains(output, "Command completed successfully!");
    }
}
