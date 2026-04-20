using D20Tek.Tools.DevKillPort.Contracts;
using D20Tek.Tools.UnitTests.DevKillPort.Fakes;

namespace D20Tek.Tools.UnitTests.DevKillPort.Commands;

[TestClass]
public class ListPortCommandTests
{
    private static readonly PortProcessInfo _testProcess =
        new(5000, 1234, "dotnet", "TCP", "0.0.0.0", PortState.Listen);

    [TestMethod]
    public async Task Execute_WithNoProcesses_ReturnsSuccessWithMessage()
    {
        // arrange
        var resolver = new FakePortResolver();
        var terminator = new FakeProcessTerminator();
        var context = TestContextFactory.CreateWithFakes(resolver, terminator);

        // act
        var result = await context.RunAsync(["list", "5000"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains("No processes found on port 5000", result.Output);
    }

    [TestMethod]
    public async Task Execute_WithProcesses_DisplaysTable()
    {
        // arrange
        var resolver = new FakePortResolver().WithResults([_testProcess]);
        var terminator = new FakeProcessTerminator();
        var context = TestContextFactory.CreateWithFakes(resolver, terminator);

        // act
        var result = await context.RunAsync(["list", "5000"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains("1234", result.Output);
        Assert.Contains("dotnet", result.Output);
    }

    [TestMethod]
    public async Task Execute_WithJsonFlag_OutputsJson()
    {
        // arrange
        var resolver = new FakePortResolver().WithResults([_testProcess]);
        var terminator = new FakeProcessTerminator();
        var context = TestContextFactory.CreateWithFakes(resolver, terminator);

        // act
        var result = await context.RunAsync(["list", "5000", "--json"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains("\"port\": 5000", result.Output);
        Assert.Contains("\"pid\": 1234", result.Output);
        Assert.Contains("\"name\": \"dotnet\"", result.Output);
    }

    [TestMethod]
    public async Task Execute_WithMultipleProcesses_DisplaysAll()
    {
        // arrange
        var process2 = new PortProcessInfo(5000, 5678, "node", "UDP", "127.0.0.1", PortState.Established);
        var resolver = new FakePortResolver().WithResults([_testProcess, process2]);
        var terminator = new FakeProcessTerminator();
        var context = TestContextFactory.CreateWithFakes(resolver, terminator);

        // act
        var result = await context.RunAsync(["list", "5000"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains("dotnet", result.Output);
        Assert.Contains("node", result.Output);
    }
}
