using D20Tek.Tools.DevKillPort.Contracts;
using D20Tek.Tools.DevKillPort.Services;
using D20Tek.Tools.UnitTests.DevKillPort.Fakes;

namespace D20Tek.Tools.UnitTests.DevKillPort.Services;

[TestClass]
public class WindowsPortResolverTests
{
    [TestMethod]
    public void ParseCsvOutput_WithValidTcpOutput_ReturnsParsedResults()
    {
        // arrange
        var output = """
            "LocalAddress","LocalPort","OwningProcess","State"
            "0.0.0.0","5000","1234","2"
            """;

        // act
        var results = WindowsPortResolver.ParseCsvOutput(output, 5000, "TCP");

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual(5000, results[0].Port);
        Assert.AreEqual(1234, results[0].ProcessId);
        Assert.AreEqual("TCP", results[0].Protocol);
        Assert.AreEqual("0.0.0.0", results[0].Address);
        Assert.AreEqual(PortState.Listen, results[0].State);
    }

    [TestMethod]
    public void ParseCsvOutput_WithEmptyOutput_ReturnsEmptyList()
    {
        // act
        var results = WindowsPortResolver.ParseCsvOutput("", 5000, "TCP");

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void ParseCsvOutput_WithHeaderOnly_ReturnsEmptyList()
    {
        // arrange
        var output = "\"LocalAddress\",\"LocalPort\",\"OwningProcess\",\"State\"";

        // act
        var results = WindowsPortResolver.ParseCsvOutput(output, 5000, "TCP");

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void ParseCsvOutput_WithMultipleRows_ReturnsAllResults()
    {
        // arrange
        var output = """
            "LocalAddress","LocalPort","OwningProcess","State"
            "0.0.0.0","5000","1234","2"
            "::","5000","1234","2"
            "0.0.0.0","5000","5678","5"
            """;

        // act
        var results = WindowsPortResolver.ParseCsvOutput(output, 5000, "TCP");

        // assert
        Assert.HasCount(2, results);
    }

    [TestMethod]
    public void ParseCsvOutput_WithInvalidPid_SkipsRow()
    {
        // arrange
        var output = """
            "LocalAddress","LocalPort","OwningProcess","State"
            "0.0.0.0","5000","notapid","2"
            """;

        // act
        var results = WindowsPortResolver.ParseCsvOutput(output, 5000, "TCP");

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void ParseCsvOutput_WithUdpNoState_DefaultsToListen()
    {
        // arrange
        var output = """
            "LocalAddress","LocalPort","OwningProcess"
            "0.0.0.0","5000","1234"
            """;

        // act
        var results = WindowsPortResolver.ParseCsvOutput(output, 5000, "UDP");

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual(PortState.Listen, results[0].State);
    }

    [TestMethod]
    [DataRow("LISTEN", 0)]
    [DataRow("2", 0)]
    [DataRow("ESTABLISHED", 1)]
    [DataRow("5", 1)]
    [DataRow("TIMEWAIT", 2)]
    [DataRow("11", 2)]
    [DataRow("CLOSEWAIT", 3)]
    [DataRow("8", 3)]
    [DataRow("UNKNOWN", 4)]
    public void ParseState_WithVariousStates_ReturnsCorrectEnum(string input, int expected)
    {
        // act
        var result = WindowsPortResolver.ParseState(input);

        // assert
        Assert.AreEqual((PortState)expected, result);
    }

    [TestMethod]
    public void ParseCsvOutput_WithTooFewFields_SkipsRow()
    {
        // arrange
        var output = """
            "LocalAddress","LocalPort","OwningProcess","State"
            "0.0.0.0","5000"
            """;

        // act
        var results = WindowsPortResolver.ParseCsvOutput(output, 5000, "TCP");

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public async Task FindAsync_WithTcpProtocol_OnlyCallsTcp()
    {
        // arrange
        var tcpOutput = """
            "LocalAddress","LocalPort","OwningProcess","State"
            "0.0.0.0","5000","1234","2"
            """;
        var commandRunner = new FakeCommandRunner()
            .WithResponse("Get-NetTCPConnection", tcpOutput)
            .WithResponse("Get-NetUDPEndpoint", """
                "LocalAddress","LocalPort","OwningProcess"
                "0.0.0.0","5000","5678"
                """);
        var resolver = new WindowsPortResolver(commandRunner);
        var options = new PortQueryOptions(ProtocolType.Tcp);

        // act
        var results = await resolver.FindAsync(5000, options);

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual("TCP", results[0].Protocol);
        Assert.AreEqual(1234, results[0].ProcessId);
    }

    [TestMethod]
    public async Task FindAsync_WithUdpProtocol_OnlyCallsUdp()
    {
        // arrange
        var udpOutput = """
            "LocalAddress","LocalPort","OwningProcess"
            "0.0.0.0","5000","5678"
            """;
        var commandRunner = new FakeCommandRunner()
            .WithResponse("Get-NetTCPConnection", """
                "LocalAddress","LocalPort","OwningProcess","State"
                "0.0.0.0","5000","1234","2"
                """)
            .WithResponse("Get-NetUDPEndpoint", udpOutput);
        var resolver = new WindowsPortResolver(commandRunner);
        var options = new PortQueryOptions(ProtocolType.Udp);

        // act
        var results = await resolver.FindAsync(5000, options);

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual("UDP", results[0].Protocol);
        Assert.AreEqual(5678, results[0].ProcessId);
    }

    [TestMethod]
    public async Task FindAsync_WithBothProtocol_CallsTcpAndUdp()
    {
        // arrange
        var commandRunner = new FakeCommandRunner()
            .WithResponse("Get-NetTCPConnection", """
                "LocalAddress","LocalPort","OwningProcess","State"
                "0.0.0.0","5000","1234","2"
                """)
            .WithResponse("Get-NetUDPEndpoint", """
                "LocalAddress","LocalPort","OwningProcess"
                "0.0.0.0","5000","5678"
                """);
        var resolver = new WindowsPortResolver(commandRunner);
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
        var commandRunner = new FakeCommandRunner()
            .WithResponse("Get-NetTCPConnection", "")
            .WithResponse("Get-NetUDPEndpoint", "");
        var resolver = new WindowsPortResolver(commandRunner);
        var options = new PortQueryOptions(ProtocolType.Both);

        // act
        var results = await resolver.FindAsync(5000, options);

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public async Task FindAsync_WithBothProtocol_DeduplicatesByPidAcrossProtocols()
    {
        // arrange
        var commandRunner = new FakeCommandRunner()
            .WithResponse("Get-NetTCPConnection", """
                "LocalAddress","LocalPort","OwningProcess","State"
                "0.0.0.0","5000","1234","2"
                """)
            .WithResponse("Get-NetUDPEndpoint", """
                "LocalAddress","LocalPort","OwningProcess"
                "0.0.0.0","5000","1234"
                """);
        var resolver = new WindowsPortResolver(commandRunner);
        var options = new PortQueryOptions(ProtocolType.Both);

        // act
        var results = await resolver.FindAsync(5000, options);

        // assert
        Assert.HasCount(2, results);
        Assert.AreEqual("TCP", results[0].Protocol);
        Assert.AreEqual("UDP", results[1].Protocol);
    }

    [TestMethod]
    public void ParseAllCsvOutput_WithValidTcpOutput_ReturnsParsedResults()
    {
        // arrange
        var output = """
            "LocalAddress","LocalPort","OwningProcess","State"
            "0.0.0.0","5000","1234","2"
            """;

        // act
        var results = WindowsPortResolver.ParseAllCsvOutput(output, "TCP");

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual(5000, results[0].Port);
        Assert.AreEqual(1234, results[0].ProcessId);
        Assert.AreEqual("TCP", results[0].Protocol);
        Assert.AreEqual("0.0.0.0", results[0].Address);
        Assert.AreEqual(PortState.Listen, results[0].State);
    }

    [TestMethod]
    public void ParseAllCsvOutput_WithEmptyOutput_ReturnsEmptyList()
    {
        // act
        var results = WindowsPortResolver.ParseAllCsvOutput("", "TCP");

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void ParseAllCsvOutput_WithHeaderOnly_ReturnsEmptyList()
    {
        // arrange
        var output = "\"LocalAddress\",\"LocalPort\",\"OwningProcess\",\"State\"";

        // act
        var results = WindowsPortResolver.ParseAllCsvOutput(output, "TCP");

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void ParseAllCsvOutput_WithMultiplePorts_ReturnsAll()
    {
        // arrange
        var output = """
            "LocalAddress","LocalPort","OwningProcess","State"
            "0.0.0.0","5000","1234","2"
            "0.0.0.0","8080","5678","5"
            """;

        // act
        var results = WindowsPortResolver.ParseAllCsvOutput(output, "TCP");

        // assert
        Assert.HasCount(2, results);
        Assert.AreEqual(5000, results[0].Port);
        Assert.AreEqual(8080, results[1].Port);
    }

    [TestMethod]
    public void ParseAllCsvOutput_WithInvalidPort_SkipsRow()
    {
        // arrange
        var output = """
            "LocalAddress","LocalPort","OwningProcess","State"
            "0.0.0.0","notaport","1234","2"
            """;

        // act
        var results = WindowsPortResolver.ParseAllCsvOutput(output, "TCP");

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void ParseAllCsvOutput_WithInvalidPid_SkipsRow()
    {
        // arrange
        var output = """
            "LocalAddress","LocalPort","OwningProcess","State"
            "0.0.0.0","5000","notapid","2"
            """;

        // act
        var results = WindowsPortResolver.ParseAllCsvOutput(output, "TCP");

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void ParseAllCsvOutput_WithTooFewFields_SkipsRow()
    {
        // arrange
        var output = """
            "LocalAddress","LocalPort","OwningProcess","State"
            "0.0.0.0","5000"
            """;

        // act
        var results = WindowsPortResolver.ParseAllCsvOutput(output, "TCP");

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void ParseAllCsvOutput_WithUdpNoState_DefaultsToListen()
    {
        // arrange
        var output = """
            "LocalAddress","LocalPort","OwningProcess"
            "0.0.0.0","5000","1234"
            """;

        // act
        var results = WindowsPortResolver.ParseAllCsvOutput(output, "UDP");

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual(PortState.Listen, results[0].State);
        Assert.AreEqual("UDP", results[0].Protocol);
    }

    [TestMethod]
    public async Task ListAllAsync_WithTcpProtocol_OnlyCallsTcp()
    {
        // arrange
        var commandRunner = new FakeCommandRunner()
            .WithResponse("Get-NetTCPConnection", """
                "LocalAddress","LocalPort","OwningProcess","State"
                "0.0.0.0","5000","1234","2"
                """)
            .WithResponse("Get-NetUDPEndpoint", """
                "LocalAddress","LocalPort","OwningProcess"
                "0.0.0.0","8080","5678"
                """);
        var resolver = new WindowsPortResolver(commandRunner);
        var options = new PortQueryOptions(ProtocolType.Tcp);

        // act
        var results = await resolver.ListAllAsync(options);

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual("TCP", results[0].Protocol);
        Assert.AreEqual(5000, results[0].Port);
    }

    [TestMethod]
    public async Task ListAllAsync_WithUdpProtocol_OnlyCallsUdp()
    {
        // arrange
        var commandRunner = new FakeCommandRunner()
            .WithResponse("Get-NetTCPConnection", """
                "LocalAddress","LocalPort","OwningProcess","State"
                "0.0.0.0","5000","1234","2"
                """)
            .WithResponse("Get-NetUDPEndpoint", """
                "LocalAddress","LocalPort","OwningProcess"
                "0.0.0.0","8080","5678"
                """);
        var resolver = new WindowsPortResolver(commandRunner);
        var options = new PortQueryOptions(ProtocolType.Udp);

        // act
        var results = await resolver.ListAllAsync(options);

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual("UDP", results[0].Protocol);
        Assert.AreEqual(8080, results[0].Port);
    }

    [TestMethod]
    public async Task ListAllAsync_WithBothProtocol_CallsTcpAndUdp()
    {
        // arrange
        var commandRunner = new FakeCommandRunner()
            .WithResponse("Get-NetTCPConnection", """
                "LocalAddress","LocalPort","OwningProcess","State"
                "0.0.0.0","5000","1234","2"
                """)
            .WithResponse("Get-NetUDPEndpoint", """
                "LocalAddress","LocalPort","OwningProcess"
                "0.0.0.0","8080","5678"
                """);
        var resolver = new WindowsPortResolver(commandRunner);
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
        var commandRunner = new FakeCommandRunner()
            .WithResponse("Get-NetTCPConnection", "")
            .WithResponse("Get-NetUDPEndpoint", "");
        var resolver = new WindowsPortResolver(commandRunner);
        var options = new PortQueryOptions(ProtocolType.Both);

        // act
        var results = await resolver.ListAllAsync(options);

        // assert
        Assert.HasCount(0, results);
    }
}
