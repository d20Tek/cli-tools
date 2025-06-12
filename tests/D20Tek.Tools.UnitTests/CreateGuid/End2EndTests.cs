using D20Tek.Spectre.Console.Extensions.Testing;
using D20Tek.Tools.CreateGuid;
using System.Diagnostics.CodeAnalysis;

namespace D20Tek.Tools.UnitTests.CreateGuid;

[TestClass]
[ExcludeFromCodeCoverage]
public sealed class End2EndTests
{
    [TestMethod]
    public async Task Run_WithDefaultArgs()
    {
        // arrange
        await Task.Delay(100);

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
}
