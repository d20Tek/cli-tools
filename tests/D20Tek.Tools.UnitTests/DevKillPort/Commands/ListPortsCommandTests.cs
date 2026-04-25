using D20Tek.Tools.DevKillPort.Commands;
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

    private static readonly PortProcessInfo _systemProcess =
        new(443, 4, "System", "TCP", "0.0.0.0", PortState.Listen);

    private static readonly PortProcessInfo _highSystemProcess =
        new(52000, 8888, "svchost", "TCP", "0.0.0.0", PortState.Established);

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

    [TestMethod]
    public async Task Execute_DefaultMinPort_ExcludesSystemPorts()
    {
        // arrange
        var resolver = new FakePortResolver().WithResults([_systemProcess, _testProcess1]);
        var terminator = new FakeProcessTerminator();
        var context = TestContextFactory.CreateWithFakes(resolver, terminator);

        // act
        var result = await context.RunAsync(["list"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains("5000", result.Output);
        Assert.IsFalse(result.Output.Contains("443"));
    }

    [TestMethod]
    public async Task Execute_WithMinPortZero_ShowsAllPorts()
    {
        // arrange
        var resolver = new FakePortResolver().WithResults([_systemProcess, _testProcess1]);
        var terminator = new FakeProcessTerminator();
        var context = TestContextFactory.CreateWithFakes(resolver, terminator);

        // act
        var result = await context.RunAsync(["list", "--min-port", "0"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains("443", result.Output);
        Assert.Contains("5000", result.Output);
    }

    [TestMethod]
    public async Task Execute_WithMaxPort_ExcludesHighPorts()
    {
        // arrange
        var resolver = new FakePortResolver().WithResults([_testProcess1, _highSystemProcess]);
        var terminator = new FakeProcessTerminator();
        var context = TestContextFactory.CreateWithFakes(resolver, terminator);

        // act
        var result = await context.RunAsync(["list", "--max-port", "49999"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains("5000", result.Output);
        Assert.IsFalse(result.Output.Contains("52000"));
    }

    [TestMethod]
    public async Task Execute_WithMinAndMaxPort_ShowsOnlyRange()
    {
        // arrange
        var resolver = new FakePortResolver()
            .WithResults([_systemProcess, _testProcess1, _testProcess2, _highSystemProcess]);
        var terminator = new FakeProcessTerminator();
        var context = TestContextFactory.CreateWithFakes(resolver, terminator);

        // act
        var result = await context.RunAsync(["list", "--min-port", "5000", "--max-port", "8080"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains("5000", result.Output);
        Assert.Contains("8080", result.Output);
        Assert.IsFalse(result.Output.Contains("443"));
        Assert.IsFalse(result.Output.Contains("52000"));
    }

    [TestMethod]
    public async Task Execute_WithPortRangeFilteringAllOut_ShowsNoPortsMessage()
    {
        // arrange
        var resolver = new FakePortResolver().WithResults([_systemProcess]);
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
    public void FilterByPortRange_WithAllInRange_ReturnsAll()
    {
        // arrange
        var processes = new List<PortProcessInfo> { _testProcess1, _testProcess2 };

        // act
        var result = ListPortsCommand.FilterByPortRange(processes, 1024, 65535);

        // assert
        Assert.HasCount(2, result);
    }

    [TestMethod]
    public void FilterByPortRange_WithNoneInRange_ReturnsEmpty()
    {
        // arrange
        var processes = new List<PortProcessInfo> { _testProcess1, _testProcess2 };

        // act
        var result = ListPortsCommand.FilterByPortRange(processes, 9000, 10000);

        // assert
        Assert.HasCount(0, result);
    }

    [TestMethod]
    public void FilterByPortRange_WithPartialMatch_ReturnsMatching()
    {
        // arrange
        var processes = new List<PortProcessInfo>
        {
            _systemProcess, _testProcess1, _testProcess2, _highSystemProcess
        };

        // act
        var result = ListPortsCommand.FilterByPortRange(processes, 5000, 9000);

        // assert
        Assert.HasCount(2, result);
        Assert.IsTrue(result.All([ExcludeFromCodeCoverage](p) => p.Port >= 5000 && p.Port <= 9000));
    }

    [TestMethod]
    public void FilterByPortRange_WithZeroMin_IncludesAll()
    {
        // arrange
        var processes = new List<PortProcessInfo> { _systemProcess, _testProcess1, _highSystemProcess };

        // act
        var result = ListPortsCommand.FilterByPortRange(processes, 0, 65535);

        // assert
        Assert.HasCount(3, result);
    }

    [TestMethod]
    public void FilterByPortRange_WithExactBoundaries_IncludesBoundaryPorts()
    {
        // arrange
        var processes = new List<PortProcessInfo> { _testProcess1, _testProcess2 };

        // act
        var result = ListPortsCommand.FilterByPortRange(processes, 5000, 8080);

        // assert
        Assert.HasCount(2, result);
    }
}
