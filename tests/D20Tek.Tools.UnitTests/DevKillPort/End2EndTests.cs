using D20Tek.Tools.DevKillPort;

namespace D20Tek.Tools.UnitTests.DevKillPort;

[TestClass]
[ExcludeFromCodeCoverage]
public sealed class End2EndTests
{
    private string _testFolder = string.Empty;

    [TestMethod]
    public async Task Run_ListCommand_WithEmptyFolder_ReturnsSuccess()
    {
        // arrange
        string[] args = ["view", "5000"];

        // act
        var result = await CommandAppE2ERunner.RunAsync(Program.Main, args);

        // assert
        Assert.AreEqual(0, result.ExitCode);
    }

    public TestContext TestContext { get; set; } = null!;
}
