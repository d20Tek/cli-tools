using D20Tek.Tools.DevKillPort.Contracts;
using D20Tek.Tools.DevKillPort.Services;
using D20Tek.Tools.UnitTests.DevKillPort.Fakes;

namespace D20Tek.Tools.UnitTests.DevKillPort.Services;

[TestClass]
public class MacPortResolverTests
{
    [TestMethod]
    public void ParseLsofOutput_WithValidOutput_ReturnsParsedResults()
    {
        // arrange
        var output = """
            COMMAND  PID  USER  FD  TYPE  DEVICE  SIZE/OFF  NODE  NAME
            dotnet   1234 root  3u  IPv4  12345   0t0       TCP   *:5000 (LISTEN)
            """;

        // act
        var results = MacPortResolver.ParseLsofOutput(output, 5000, "");

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual(1234, results[0].ProcessId);
        Assert.AreEqual("dotnet", results[0].ProcessName);
        Assert.AreEqual("TCP", results[0].Protocol);
        Assert.AreEqual(PortState.Listen, results[0].State);
    }

    [TestMethod]
    public void ParseLsofOutput_WithEmptyOutput_ReturnsEmptyList()
    {
        // act
        var results = MacPortResolver.ParseLsofOutput("", 5000, "");

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void ParseLsofOutput_WithProtocolFilter_FiltersResults()
    {
        // arrange
        var output = """
            COMMAND  PID  USER  FD  TYPE  DEVICE  SIZE/OFF  NODE  NAME
            dotnet   1234 root  3u  IPv4  12345   0t0       UDP   *:5000
            """;

        // act
        var results = MacPortResolver.ParseLsofOutput(output, 5000, "TCP");

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void ParseLsofOutput_WithEstablishedState_ParsesCorrectly()
    {
        // arrange
        var output = """
            COMMAND  PID  USER  FD  TYPE  DEVICE  SIZE/OFF  NODE  NAME
            dotnet   1234 root  3u  IPv4  12345   0t0       TCP   127.0.0.1:5000->127.0.0.1:52000 (ESTABLISHED)
            """;

        // act
        var results = MacPortResolver.ParseLsofOutput(output, 5000, "");

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual(PortState.Established, results[0].State);
    }

    [TestMethod]
    public void ParseLsofOutput_WithDuplicatePids_DeduplicatesResults()
    {
        // arrange
        var output = """
            COMMAND  PID  USER  FD  TYPE  DEVICE  SIZE/OFF  NODE  NAME
            dotnet   1234 root  3u  IPv4  12345   0t0       TCP   *:5000 (LISTEN)
            dotnet   1234 root  4u  IPv4  12346   0t0       TCP   *:5000 (LISTEN)
            """;

        // act
        var results = MacPortResolver.ParseLsofOutput(output, 5000, "");

        // assert
        Assert.HasCount(1, results);
    }

    [TestMethod]
    public void ParseLsofOutput_WithInvalidPid_SkipsRow()
    {
        // arrange
        var output = """
            COMMAND  PID    USER  FD  TYPE  DEVICE  SIZE/OFF  NODE  NAME
            dotnet   notpid root  3u  IPv4  12345   0t0       TCP   *:5000 (LISTEN)
            """;

        // act
        var results = MacPortResolver.ParseLsofOutput(output, 5000, "");

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void ParseLsofOutput_WithOtherState_ParsesAsOther()
    {
        // arrange
        var output = """
            COMMAND  PID  USER  FD  TYPE  DEVICE  SIZE/OFF  NODE  NAME
            dotnet   1234 root  3u  IPv4  12345   0t0       TCP   127.0.0.1:5000->127.0.0.1:52000 (CLOSE_WAIT)
            """;

        // act
        var results = MacPortResolver.ParseLsofOutput(output, 5000, "");

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual(PortState.Other, results[0].State);
    }

    [TestMethod]
    public void ParseLsofOutput_WithTooFewColumns_SkipsRow()
    {
        // arrange
        var output = """
            COMMAND  PID  USER  FD  TYPE  DEVICE  SIZE/OFF  NODE  NAME
            dotnet   1234 root  3u
            """;

        // act
        var results = MacPortResolver.ParseLsofOutput(output, 5000, "");

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void ParseLsofOutput_WithHeaderOnly_ReturnsEmptyList()
    {
        // arrange
        var output = "COMMAND  PID  USER  FD  TYPE  DEVICE  SIZE/OFF  NODE  NAME";

        // act
        var results = MacPortResolver.ParseLsofOutput(output, 5000, "");

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public async Task FindAsync_WithTcpProtocol_PassesTcpFilter()
    {
        // arrange
        var lsofOutput = """
            COMMAND  PID  USER  FD  TYPE  DEVICE  SIZE/OFF  NODE  NAME
            dotnet   1234 root  3u  IPv4  12345   0t0       TCP   *:5000 (LISTEN)
            dotnet   5678 root  4u  IPv4  12346   0t0       UDP   *:5000
            """;
        var commandRunner = new FakeCommandRunner().WithResponse("lsof", lsofOutput);
        var resolver = new MacPortResolver(commandRunner);
        var options = new PortQueryOptions(ProtocolType.Tcp);

        // act
        var results = await resolver.FindAsync(5000, options);

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual("TCP", results[0].Protocol);
    }

    [TestMethod]
    public async Task FindAsync_WithUdpProtocol_PassesUdpFilter()
    {
        // arrange
        var lsofOutput = """
            COMMAND  PID  USER  FD  TYPE  DEVICE  SIZE/OFF  NODE  NAME
            dotnet   1234 root  3u  IPv4  12345   0t0       TCP   *:5000 (LISTEN)
            dotnet   5678 root  4u  IPv4  12346   0t0       UDP   *:5000
            """;
        var commandRunner = new FakeCommandRunner().WithResponse("lsof", lsofOutput);
        var resolver = new MacPortResolver(commandRunner);
        var options = new PortQueryOptions(ProtocolType.Udp);

        // act
        var results = await resolver.FindAsync(5000, options);

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual("UDP", results[0].Protocol);
    }

    [TestMethod]
    public async Task FindAsync_WithBothProtocol_ReturnsAll()
    {
        // arrange
        var lsofOutput = """
            COMMAND  PID  USER  FD  TYPE  DEVICE  SIZE/OFF  NODE  NAME
            dotnet   1234 root  3u  IPv4  12345   0t0       TCP   *:5000 (LISTEN)
            dotnet   5678 root  4u  IPv4  12346   0t0       UDP   *:5000
            """;
        var commandRunner = new FakeCommandRunner().WithResponse("lsof", lsofOutput);
        var resolver = new MacPortResolver(commandRunner);
        var options = new PortQueryOptions(ProtocolType.Both);

        // act
        var results = await resolver.FindAsync(5000, options);

        // assert
        Assert.HasCount(2, results);
    }

    [TestMethod]
    public async Task FindAsync_WithEmptyOutput_ReturnsEmptyList()
    {
        // arrange
        var commandRunner = new FakeCommandRunner().WithResponse("lsof", "");
        var resolver = new MacPortResolver(commandRunner);
        var options = new PortQueryOptions();

        // act
        var results = await resolver.FindAsync(5000, options);

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void ParseLsofAllOutput_WithValidOutput_ReturnsParsedResults()
    {
        // arrange
        var output = """
            COMMAND  PID  USER  FD  TYPE  DEVICE  SIZE/OFF  NODE  NAME       STATE
            dotnet   1234 root  3u  IPv4  12345   0t0       TCP   *:5000     (LISTEN)
            """;

        // act
        var results = MacPortResolver.ParseLsofAllOutput(output, "");

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual(5000, results[0].Port);
        Assert.AreEqual(1234, results[0].ProcessId);
        Assert.AreEqual("dotnet", results[0].ProcessName);
        Assert.AreEqual("TCP", results[0].Protocol);
        Assert.AreEqual(PortState.Listen, results[0].State);
    }

    [TestMethod]
    public void ParseLsofAllOutput_WithEmptyOutput_ReturnsEmptyList()
    {
        // act
        var results = MacPortResolver.ParseLsofAllOutput("", "");

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void ParseLsofAllOutput_WithHeaderOnly_ReturnsEmptyList()
    {
        // arrange
        var output = "COMMAND  PID  USER  FD  TYPE  DEVICE  SIZE/OFF  NODE  NAME";

        // act
        var results = MacPortResolver.ParseLsofAllOutput(output, "");

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void ParseLsofAllOutput_WithProtocolFilter_FiltersResults()
    {
        // arrange
        var output = """
            COMMAND  PID  USER  FD  TYPE  DEVICE  SIZE/OFF  NODE  NAME       STATE
            dotnet   1234 root  3u  IPv4  12345   0t0       UDP   *:5000     (LISTEN)
            """;

        // act
        var results = MacPortResolver.ParseLsofAllOutput(output, "TCP");

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void ParseLsofAllOutput_WithEstablishedState_ParsesCorrectly()
    {
        // arrange
        var output = """
            COMMAND  PID  USER  FD  TYPE  DEVICE  SIZE/OFF  NODE  NAME                              STATE
            dotnet   1234 root  3u  IPv4  12345   0t0       TCP   127.0.0.1:5000->127.0.0.1:52000  (ESTABLISHED)
            """;

        // act
        var results = MacPortResolver.ParseLsofAllOutput(output, "");

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual(PortState.Established, results[0].State);
    }

    [TestMethod]
    public void ParseLsofAllOutput_WithOtherState_WhenNinePartsOnly_ParsesAsOther()
    {
        // arrange - exactly 9 parts, no state token
        var output = """
            COMMAND  PID  USER  FD  TYPE  DEVICE  SIZE/OFF  NODE  NAME
            dotnet   1234 root  3u  IPv4  12345   0t0       TCP   *:5000
            """;

        // act
        var results = MacPortResolver.ParseLsofAllOutput(output, "");

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual(PortState.Other, results[0].State);
    }

    [TestMethod]
    public void ParseLsofAllOutput_WithUnknownState_ParsesAsOther()
    {
        // arrange
        var output = """
            COMMAND  PID  USER  FD  TYPE  DEVICE  SIZE/OFF  NODE  NAME       STATE
            dotnet   1234 root  3u  IPv4  12345   0t0       TCP   *:5000     (CLOSE_WAIT)
            """;

        // act
        var results = MacPortResolver.ParseLsofAllOutput(output, "");

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual(PortState.Other, results[0].State);
    }

    [TestMethod]
    public void ParseLsofAllOutput_WithTooFewParts_SkipsRow()
    {
        // arrange
        var output = """
            COMMAND  PID  USER  FD  TYPE  DEVICE  SIZE/OFF  NODE  NAME
            dotnet   1234 root  3u
            """;

        // act
        var results = MacPortResolver.ParseLsofAllOutput(output, "");

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void ParseLsofAllOutput_WithInvalidPid_SkipsRow()
    {
        // arrange
        var output = """
            COMMAND  PID    USER  FD  TYPE  DEVICE  SIZE/OFF  NODE  NAME
            dotnet   notpid root  3u  IPv4  12345   0t0       TCP   *:5000
            """;

        // act
        var results = MacPortResolver.ParseLsofAllOutput(output, "");

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void ParseLsofAllOutput_WithDuplicatePidAndPort_DeduplicatesResults()
    {
        // arrange
        var output = """
            COMMAND  PID  USER  FD  TYPE  DEVICE  SIZE/OFF  NODE  NAME
            dotnet   1234 root  3u  IPv4  12345   0t0       TCP   *:5000
            dotnet   1234 root  4u  IPv4  12346   0t0       TCP   *:5000
            """;

        // act
        var results = MacPortResolver.ParseLsofAllOutput(output, "");

        // assert
        Assert.HasCount(1, results);
    }

    [TestMethod]
    public void ParseLsofAllOutput_WithMultiplePorts_ReturnsAll()
    {
        // arrange
        var output = """
            COMMAND  PID  USER  FD  TYPE  DEVICE  SIZE/OFF  NODE  NAME
            dotnet   1234 root  3u  IPv4  12345   0t0       TCP   *:5000
            node     5678 root  4u  IPv4  12346   0t0       TCP   *:8080
            """;

        // act
        var results = MacPortResolver.ParseLsofAllOutput(output, "");

        // assert
        Assert.HasCount(2, results);
        Assert.AreEqual(5000, results[0].Port);
        Assert.AreEqual(8080, results[1].Port);
    }

    [TestMethod]
    public void ParseLsofAllOutput_WithNoColonInNameField_ExtractsZeroPort()
    {
        // arrange
        var output = """
            COMMAND  PID  USER  FD  TYPE  DEVICE  SIZE/OFF  NODE  NAME
            dotnet   1234 root  3u  IPv4  12345   0t0       TCP   nocolon
            """;

        // act
        var results = MacPortResolver.ParseLsofAllOutput(output, "");

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual(0, results[0].Port);
    }

    [TestMethod]
    public async Task ListAllAsync_WithTcpProtocol_PassesTcpFilter()
    {
        // arrange
        var lsofOutput = """
            COMMAND  PID  USER  FD  TYPE  DEVICE  SIZE/OFF  NODE  NAME
            dotnet   1234 root  3u  IPv4  12345   0t0       TCP   *:5000
            node     5678 root  4u  IPv4  12346   0t0       UDP   *:8080
            """;
        var commandRunner = new FakeCommandRunner().WithResponse("lsof", lsofOutput);
        var resolver = new MacPortResolver(commandRunner);
        var options = new PortQueryOptions(ProtocolType.Tcp);

        // act
        var results = await resolver.ListAllAsync(options);

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual("TCP", results[0].Protocol);
    }

    [TestMethod]
    public async Task ListAllAsync_WithUdpProtocol_PassesUdpFilter()
    {
        // arrange
        var lsofOutput = """
            COMMAND  PID  USER  FD  TYPE  DEVICE  SIZE/OFF  NODE  NAME
            dotnet   1234 root  3u  IPv4  12345   0t0       TCP   *:5000
            node     5678 root  4u  IPv4  12346   0t0       UDP   *:8080
            """;
        var commandRunner = new FakeCommandRunner().WithResponse("lsof", lsofOutput);
        var resolver = new MacPortResolver(commandRunner);
        var options = new PortQueryOptions(ProtocolType.Udp);

        // act
        var results = await resolver.ListAllAsync(options);

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual("UDP", results[0].Protocol);
    }

    [TestMethod]
    public async Task ListAllAsync_WithBothProtocol_ReturnsAll()
    {
        // arrange
        var lsofOutput = """
            COMMAND  PID  USER  FD  TYPE  DEVICE  SIZE/OFF  NODE  NAME
            dotnet   1234 root  3u  IPv4  12345   0t0       TCP   *:5000
            node     5678 root  4u  IPv4  12346   0t0       UDP   *:8080
            """;
        var commandRunner = new FakeCommandRunner().WithResponse("lsof", lsofOutput);
        var resolver = new MacPortResolver(commandRunner);
        var options = new PortQueryOptions(ProtocolType.Both);

        // act
        var results = await resolver.ListAllAsync(options);

        // assert
        Assert.HasCount(2, results);
    }

    [TestMethod]
    public async Task ListAllAsync_WithEmptyOutput_ReturnsEmptyList()
    {
        // arrange
        var commandRunner = new FakeCommandRunner().WithResponse("lsof", "");
        var resolver = new MacPortResolver(commandRunner);
        var options = new PortQueryOptions(ProtocolType.Both);

        // act
        var results = await resolver.ListAllAsync(options);

        // assert
        Assert.HasCount(0, results);
    }
}
