using D20Tek.Tools.CreateGuid;

namespace D20Tek.Tools.UnitTests.CreateGuid;

[TestClass]
[ExcludeFromCodeCoverage]
public sealed class End2EndTests
{
    [TestMethod]
    public async Task Run_WithDefaultArgs()
    {
        // arrange
        await Task.Delay(100, TestContext.CancellationToken);

        // act
        var result = await CommandAppE2ERunner.RunAsync(Program.Main, []);

        // assert
        Assert.AreEqual(0, result.ExitCode);
    }

    [TestMethod]
    public async Task Run_WithCountArgs()
    {
        // arrange
        string[] args = ["--count", "10"];

        // act
        var result = await CommandAppE2ERunner.RunAsync(Program.Main, args);

        // assert
        Assert.AreEqual(0, result.ExitCode);
    }

    public TestContext TestContext { get; set; }
}
