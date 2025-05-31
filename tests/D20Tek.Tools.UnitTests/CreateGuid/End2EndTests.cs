using D20Tek.Tools.CreateGuid;
using Spectre.Console;

namespace D20Tek.Tools.UnitTests.CreateGuid;

[TestClass]
[TestCategory("E2E")]
public class End2EndTests
{
    [TestMethod]
    public async Task Run_WithDefaultArgs()
    {
        // arrange
        var args = Array.Empty<string>();
        AnsiConsole.Record();

        // act
        var result = await Program.Main(args);

        // assert
        Assert.AreEqual(0, result);
        var output = AnsiConsole.ExportText();
        StringAssert.StartsWith(output, "create-guid: running");
        StringAssert.Contains(output, "Command completed successfully!");
    }

    [TestMethod]
    public async Task Run_WithCountArgs()
    {
        // arrange
        var args = new string[] { "--count", "10" };
        AnsiConsole.Record();

        // act
        var result = await Program.Main(args);

        // assert
        Assert.AreEqual(0, result);
        var output = AnsiConsole.ExportText();
        StringAssert.StartsWith(output, "create-guid: running");
        StringAssert.Contains(output, "Command completed successfully!");
    }
}
