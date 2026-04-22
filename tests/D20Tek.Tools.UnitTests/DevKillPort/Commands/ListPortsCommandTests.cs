using D20Tek.Tools.DevKillPort.Contracts;
using D20Tek.Tools.UnitTests.DevKillPort.Fakes;

namespace D20Tek.Tools.UnitTests.DevKillPort.Commands;

[TestClass]
public class ListPortsCommandTests
{
    private static readonly PortProcessInfo _testProcess1 =
        new(5000, 1234, "dotnet", "TCP", "0.0.0.0", PortState.Listen);

    private static readonly PortProcessInfo _testProcess2 =
        new(8080, 5678, "node", "TCP", "127.0.0.1", PortState.Established);

    private static readonly PortProcessInfo _udpProcess =
        new(9000, 9999, "udpapp", "UDP", "0.0.0.0", PortState.Listen);

    [TestMethod]
    public async Task Execute_WithNoProcesses_ReturnsSuccessWithMessage()
    {
        // arrange
        var resolver = new FakePortResolver();
        var terminator = new FakeProcessTerminator();
        var context = TestContextFactory.CreateWithFakes(resolver, terminator);

        // act
        var result = await context.RunAsync(["list"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains("No active port bindings found", result.Output);
    }

    [TestMethod]
    public async Task Execute_WithProcesses_DisplaysTable()
    {
        // arrange
        var resolver = new FakePortResolver().WithResults([_testProcess1, _testProcess2]);
        var terminator = new FakeProcessTerminator();
        var context = TestContextFactory.CreateWithFakes(resolver, terminator);

        // act
        var result = await context.RunAsync(["list"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains("5000", result.Output);
        Assert.Contains("8080", result.Output);
        Assert.Contains("dotnet", result.Output);
        Assert.Contains("node", result.Output);
    }

    [TestMethod]
    public async Task Execute_WithJsonFlag_OutputsJson()
    {
        // arrange
        var resolver = new FakePortResolver().WithResults([_testProcess1, _testProcess2]);
        var terminator = new FakeProcessTerminator();
        var context = TestContextFactory.CreateWithFakes(resolver, terminator);

        // act
        var result = await context.RunAsync(["list", "--json"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains("\"port\": 5000", result.Output);
        Assert.Contains("\"pid\": 1234", result.Output);
        Assert.Contains("\"port\": 8080", result.Output);
        Assert.Contains("\"pid\": 5678", result.Output);
    }

    [TestMethod]
    public async Task Execute_WithProtocolFilter_DisplaysFilteredResults()
    {
        // arrange
        var resolver = new FakePortResolver().WithResults([_testProcess1]);
        var terminator = new FakeProcessTerminator();
        var context = TestContextFactory.CreateWithFakes(resolver, terminator);

        // act
        var result = await context.RunAsync(["list", "--protocol", "tcp"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains("dotnet", result.Output);
    }

    [TestMethod]
    public async Task Execute_WithUdpProtocolFilter_DisplaysUdpResults()
    {
        // arrange
        var resolver = new FakePortResolver().WithResults([_udpProcess]);
        var terminator = new FakeProcessTerminator();
        var context = TestContextFactory.CreateWithFakes(resolver, terminator);

        // act
        var result = await context.RunAsync(["list", "--protocol", "udp"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains("udpapp", result.Output);
        Assert.Contains("9000", result.Output);
    }
}
