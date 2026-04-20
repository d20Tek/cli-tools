using D20Tek.Tools.DevKillPort;
using D20Tek.Tools.DevKillPort.Contracts;
using D20Tek.Tools.UnitTests.DevKillPort.Fakes;

namespace D20Tek.Tools.UnitTests.DevKillPort.Commands;

[TestClass]
public class KillPortCommandTests
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
        var result = await context.RunAsync(["kill", "5000"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains("No processes found on port 5000", result.Output);
    }

    [TestMethod]
    public async Task Execute_WithForceFlag_KillsWithoutPrompt()
    {
        // arrange
        var resolver = new FakePortResolver().WithResults([_testProcess]);
        var terminator = new FakeProcessTerminator();
        var context = TestContextFactory.CreateWithFakes(resolver, terminator);

        // act
        var result = await context.RunAsync(["kill", "5000", "--force"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains("Successfully killed", result.Output);
        Assert.AreEqual(1, terminator.KillCallCount);
    }

    [TestMethod]
    public async Task Execute_WithDryRun_DoesNotKill()
    {
        // arrange
        var resolver = new FakePortResolver().WithResults([_testProcess]);
        var terminator = new FakeProcessTerminator();
        var context = TestContextFactory.CreateWithFakes(resolver, terminator);

        // act
        var result = await context.RunAsync(["kill", "5000", "--dry-run"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains("Dry run", result.Output);
        Assert.AreEqual(0, terminator.KillCallCount);
    }

    [TestMethod]
    public async Task Execute_WithJsonFlag_OutputsJson()
    {
        // arrange
        var resolver = new FakePortResolver().WithResults([_testProcess]);
        var terminator = new FakeProcessTerminator();
        var context = TestContextFactory.CreateWithFakes(resolver, terminator);

        // act
        var result = await context.RunAsync(["kill", "5000", "--json", "--dry-run"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains("\"port\": 5000", result.Output);
        Assert.Contains("\"pid\": 1234", result.Output);
    }

    [TestMethod]
    public async Task Execute_WithForceAndAll_KillsAllProcesses()
    {
        // arrange
        var process2 = new PortProcessInfo(5000, 5678, "node", "TCP", "0.0.0.0", PortState.Listen);
        var resolver = new FakePortResolver().WithResults([_testProcess, process2]);
        var terminator = new FakeProcessTerminator();
        var context = TestContextFactory.CreateWithFakes(resolver, terminator);

        // act
        var result = await context.RunAsync(["kill", "5000", "--force", "--all"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.AreEqual(2, terminator.KillCallCount);
    }

    [TestMethod]
    public async Task Execute_WithForceNoAll_KillsOnlyFirst()
    {
        // arrange
        var process2 = new PortProcessInfo(5000, 5678, "node", "TCP", "0.0.0.0", PortState.Listen);
        var resolver = new FakePortResolver().WithResults([_testProcess, process2]);
        var terminator = new FakeProcessTerminator();
        var context = TestContextFactory.CreateWithFakes(resolver, terminator);

        // act
        var result = await context.RunAsync(["kill", "5000", "--force"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.AreEqual(1, terminator.KillCallCount);
        Assert.AreEqual(1234, terminator.KilledPids[0]);
    }

    [TestMethod]
    public async Task Execute_WithKillFailure_ReportsError()
    {
        // arrange
        var resolver = new FakePortResolver().WithResults([_testProcess]);
        var terminator = new FakeProcessTerminator().WithKillResult(false);
        var context = TestContextFactory.CreateWithFakes(resolver, terminator);

        // act
        var result = await context.RunAsync(["kill", "5000", "--force"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains("Failed to kill", result.Output);
    }

    [TestMethod]
    public async Task Execute_WithConfirmationDeclined_DoesNotKill()
    {
        // arrange
        var resolver = new FakePortResolver().WithResults([_testProcess]);
        var terminator = new FakeProcessTerminator();
        var context = TestContextFactory.CreateWithFakes(resolver, terminator);
        context.Console.TestInput.PushTextWithEnter("n");

        // act
        var result = await context.RunAsync(["kill", "5000"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.AreEqual(0, terminator.KillCallCount);
    }

    [TestMethod]
    public async Task Execute_WithConfirmationAccepted_KillsProcess()
    {
        // arrange
        var resolver = new FakePortResolver().WithResults([_testProcess]);
        var terminator = new FakeProcessTerminator();
        var context = TestContextFactory.CreateWithFakes(resolver, terminator);
        context.Console.TestInput.PushTextWithEnter("y");

        // act
        var result = await context.RunAsync(["kill", "5000"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.AreEqual(1, terminator.KillCallCount);
    }

    [TestMethod]
    public async Task Execute_WithWatchAndPortFreed_ReturnsSuccess()
    {
        // arrange
        var resolver = new FakePortResolver()
            .ThenReturns([_testProcess])
            .ThenReturns([]);
        var terminator = new FakeProcessTerminator();
        var context = TestContextFactory.CreateWithFakes(resolver, terminator);

        // act
        var result = await context.RunAsync(["kill", "5000", "--force", "--watch", "--timeout", "3"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains("Successfully killed", result.Output);
        Assert.Contains("now free", result.Output);
        Assert.AreEqual(2, resolver.CallCount);
    }

    [TestMethod]
    public async Task Execute_WithWatchAndTimeout_ReturnsError()
    {
        // arrange
        var resolver = new FakePortResolver().WithResults([_testProcess]);
        var terminator = new FakeProcessTerminator();
        var context = TestContextFactory.CreateWithFakes(resolver, terminator);

        // act
        var result = await context.RunAsync(["kill", "5000", "--force", "--watch", "--timeout", "1"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(-1, result.ExitCode);
        Assert.Contains("Timeout", result.Output);
    }
}
