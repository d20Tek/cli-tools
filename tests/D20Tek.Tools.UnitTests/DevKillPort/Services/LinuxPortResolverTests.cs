using D20Tek.Tools.DevKillPort.Contracts;
using D20Tek.Tools.DevKillPort.Services;
using D20Tek.Tools.UnitTests.DevKillPort.Fakes;

namespace D20Tek.Tools.UnitTests.DevKillPort.Services;

[TestClass]
public class LinuxPortResolverTests
{
    [TestMethod]
    public void ParseSsOutput_WithValidOutput_ReturnsParsedResults()
    {
        // arrange
        var output = """
            State   Recv-Q  Send-Q  Local Address:Port  Peer Address:Port  Process
            LISTEN  0       128     0.0.0.0:5000        0.0.0.0:*          users:(("dotnet",pid=1234,fd=3))
            """;

        // act
        var results = LinuxPortResolver.ParseSsOutput(output, 5000);

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual(5000, results[0].Port);
        Assert.AreEqual(1234, results[0].ProcessId);
        Assert.AreEqual("dotnet", results[0].ProcessName);
        Assert.AreEqual(PortState.Listen, results[0].State);
    }

    [TestMethod]
    public void ParseSsOutput_WithEmptyOutput_ReturnsEmptyList()
    {
        // act
        var results = LinuxPortResolver.ParseSsOutput("", 5000);

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void ParseSsOutput_WithHeaderOnly_ReturnsEmptyList()
    {
        // arrange
        var output = "State   Recv-Q  Send-Q  Local Address:Port  Peer Address:Port  Process";

        // act
        var results = LinuxPortResolver.ParseSsOutput(output, 5000);

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void ParseSsOutput_WithDifferentPort_ReturnsEmptyList()
    {
        // arrange
        var output = """
            State   Recv-Q  Send-Q  Local Address:Port  Peer Address:Port  Process
            LISTEN  0       128     0.0.0.0:3000        0.0.0.0:*          users:(("node",pid=999,fd=3))
            """;

        // act
        var results = LinuxPortResolver.ParseSsOutput(output, 5000);

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void ParseLsofOutput_WithValidOutput_ReturnsParsedResults()
    {
        // arrange
        var output = """
            COMMAND  PID  USER  FD  TYPE  DEVICE  SIZE/OFF  NODE  NAME
            dotnet   1234 root  3u  IPv4  12345   0t0       TCP   *:5000 (LISTEN)
            """;

        // act
        var results = LinuxPortResolver.ParseLsofOutput(output, 5000, "");

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual(1234, results[0].ProcessId);
        Assert.AreEqual("dotnet", results[0].ProcessName);
        Assert.AreEqual(PortState.Listen, results[0].State);
    }

    [TestMethod]
    public void ParseLsofOutput_WithEmptyOutput_ReturnsEmptyList()
    {
        // act
        var results = LinuxPortResolver.ParseLsofOutput("", 5000, "");

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
        var results = LinuxPortResolver.ParseLsofOutput(output, 5000, "TCP");

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    [DataRow("LISTEN", 0)]
    [DataRow("ESTAB", 1)]
    [DataRow("ESTABLISHED", 1)]
    [DataRow("TIME-WAIT", 2)]
    [DataRow("TIME_WAIT", 2)]
    [DataRow("CLOSE-WAIT", 3)]
    [DataRow("UNKNOWN", 4)]
    public void ParseState_WithVariousStates_ReturnsCorrectEnum(string input, int expected)
    {
        // act
        var result = LinuxPortResolver.ParseState(input);

        // assert
        Assert.AreEqual((PortState)expected, result);
    }

    [TestMethod]
    public void ParseSsOutput_WithTooFewParts_SkipsRow()
    {
        // arrange
        var output = """
            State   Recv-Q  Send-Q  Local Address:Port  Peer Address:Port  Process
            LISTEN  0       128
            """;

        // act
        var results = LinuxPortResolver.ParseSsOutput(output, 5000);

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void ParseSsOutput_WithNoColonInAddress_SkipsRow()
    {
        // arrange
        var output = """
            State   Recv-Q  Send-Q  Local Address:Port  Peer Address:Port  Process
            LISTEN  0       128     nocolonhere         0.0.0.0:*          users:(("dotnet",pid=1234,fd=3))
            """;

        // act
        var results = LinuxPortResolver.ParseSsOutput(output, 5000);

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void ParseSsOutput_WithNonNumericPort_SkipsRow()
    {
        // arrange
        var output = """
            State   Recv-Q  Send-Q  Local Address:Port  Peer Address:Port  Process
            LISTEN  0       128     0.0.0.0:abc         0.0.0.0:*          users:(("dotnet",pid=1234,fd=3))
            """;

        // act
        var results = LinuxPortResolver.ParseSsOutput(output, 5000);

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void ParseSsOutput_WithNoPidField_SkipsRow()
    {
        // arrange
        var output = """
            State   Recv-Q  Send-Q  Local Address:Port  Peer Address:Port
            LISTEN  0       128     0.0.0.0:5000        0.0.0.0:*
            """;

        // act
        var results = LinuxPortResolver.ParseSsOutput(output, 5000);

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void ParseSsOutput_WithNoPidInProcessField_SkipsRow()
    {
        // arrange
        var output = """
            State   Recv-Q  Send-Q  Local Address:Port  Peer Address:Port  Process
            LISTEN  0       128     0.0.0.0:5000        0.0.0.0:*          users:((nopidhere))
            """;

        // act
        var results = LinuxPortResolver.ParseSsOutput(output, 5000);

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void ParseSsOutput_WithDuplicatePids_DeduplicatesResults()
    {
        // arrange
        var output = """
            State   Recv-Q  Send-Q  Local Address:Port  Peer Address:Port  Process
            LISTEN  0       128     0.0.0.0:5000        0.0.0.0:*          users:(("dotnet",pid=1234,fd=3))
            LISTEN  0       128     [::]:5000            [::]:*             users:(("dotnet",pid=1234,fd=4))
            """;

        // act
        var results = LinuxPortResolver.ParseSsOutput(output, 5000);

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual(1234, results[0].ProcessId);
    }

    [TestMethod]
    public void ParseSsOutput_WithNoProcessNameMarker_ReturnsUnknown()
    {
        // arrange
        var output = """
            State   Recv-Q  Send-Q  Local Address:Port  Peer Address:Port  Process
            LISTEN  0       128     0.0.0.0:5000        0.0.0.0:*          pid=1234,fd=3
            """;

        // act
        var results = LinuxPortResolver.ParseSsOutput(output, 5000);

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual("<unknown>", results[0].ProcessName);
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
        var results = LinuxPortResolver.ParseLsofOutput(output, 5000, "");

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual(PortState.Established, results[0].State);
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
        var results = LinuxPortResolver.ParseLsofOutput(output, 5000, "");

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual(PortState.Other, results[0].State);
    }

    [TestMethod]
    public void ParseLsofOutput_WithNonIpvType_UsesTypeAsProtocol()
    {
        // arrange
        var output = """
            COMMAND  PID  USER  FD  TCP   DEVICE  SIZE/OFF  NODE  NAME
            dotnet   1234 root  3u  TCP   12345   0t0       TCP   *:5000 (LISTEN)
            """;

        // act
        var results = LinuxPortResolver.ParseLsofOutput(output, 5000, "");

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual("TCP", results[0].Protocol);
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
        var results = LinuxPortResolver.ParseLsofOutput(output, 5000, "");

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void ParseLsofOutput_WithTooFewParts_SkipsRow()
    {
        // arrange
        var output = """
            COMMAND  PID  USER  FD  TYPE  DEVICE  SIZE/OFF  NODE  NAME
            dotnet   1234 root  3u
            """;

        // act
        var results = LinuxPortResolver.ParseLsofOutput(output, 5000, "");

        // assert
        Assert.HasCount(0, results);
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
        var results = LinuxPortResolver.ParseLsofOutput(output, 5000, "");

        // assert
        Assert.HasCount(1, results);
    }

    [TestMethod]
    public async Task FindAsync_WithSsResults_ReturnsSsResults()
    {
        // arrange
        var ssOutput = """
            State   Recv-Q  Send-Q  Local Address:Port  Peer Address:Port  Process
            LISTEN  0       128     0.0.0.0:5000        0.0.0.0:*          users:(("dotnet",pid=1234,fd=3))
            """;
        var commandRunner = new FakeCommandRunner()
            .WithResponse("ss", ssOutput);
        var procFs = new FakeProcFileSystem();
        var resolver = new LinuxPortResolver(commandRunner, procFs);
        var options = new PortQueryOptions(ProtocolType.Both);

        // act
        var results = await resolver.FindAsync(5000, options);

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual(1234, results[0].ProcessId);
    }

    [TestMethod]
    public async Task FindAsync_WithSsEmpty_FallsBackToLsof()
    {
        // arrange
        var lsofOutput = """
            COMMAND  PID  USER  FD  TYPE  DEVICE  SIZE/OFF  NODE  NAME
            dotnet   5678 root  3u  IPv4  12345   0t0       TCP   *:5000 (LISTEN)
            """;
        var commandRunner = new FakeCommandRunner()
            .WithResponse("ss", "")
            .WithResponse("lsof", lsofOutput);
        var procFs = new FakeProcFileSystem();
        var resolver = new LinuxPortResolver(commandRunner, procFs);
        var options = new PortQueryOptions(ProtocolType.Both);

        // act
        var results = await resolver.FindAsync(5000, options);

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual(5678, results[0].ProcessId);
    }

    [TestMethod]
    public async Task FindAsync_WithSsAndLsofEmpty_FallsBackToProc()
    {
        // arrange
        var commandRunner = new FakeCommandRunner()
            .WithResponse("ss", "")
            .WithResponse("lsof", "");
        var procFs = new FakeProcFileSystem()
            .WithFile("/proc/net/tcp",
            [
                "  sl  local_address rem_address   st tx_queue rx_queue tr tm->when retrnsmt   uid  timeout inode",
                "   0: 00000000:1388 00000000:0000 0A 00000000:00000000 00:00000000 00000000     0        0 12345 1 0000000000000000 100 0 0 10 0"
            ])
            .WithPidDirectory("/proc/9999")
            .WithFile("/proc/9999/comm", ["myapp"])
            .WithFdLink("/proc/9999", "/proc/9999/fd/3", "socket:[12345]");
        var resolver = new LinuxPortResolver(commandRunner, procFs);
        var options = new PortQueryOptions(ProtocolType.Both);

        // act
        var results = await resolver.FindAsync(5000, options);

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual(9999, results[0].ProcessId);
        Assert.AreEqual("myapp", results[0].ProcessName);
    }

    [TestMethod]
    public async Task FindAsync_WithTcpProtocol_PassesTcpFlagToSs()
    {
        // arrange
        var ssOutput = """
            State   Recv-Q  Send-Q  Local Address:Port  Peer Address:Port  Process
            LISTEN  0       128     0.0.0.0:5000        0.0.0.0:*          users:(("dotnet",pid=1234,fd=3))
            """;
        var commandRunner = new FakeCommandRunner()
            .WithResponse("ss", ssOutput);
        var procFs = new FakeProcFileSystem();
        var resolver = new LinuxPortResolver(commandRunner, procFs);
        var options = new PortQueryOptions(ProtocolType.Tcp);

        // act
        var results = await resolver.FindAsync(5000, options);

        // assert
        Assert.HasCount(1, results);
    }

    [TestMethod]
    public async Task FindAsync_WithUdpProtocol_PassesUdpFlagToSs()
    {
        // arrange
        var commandRunner = new FakeCommandRunner()
            .WithResponse("ss", "");
        var procFs = new FakeProcFileSystem();
        var resolver = new LinuxPortResolver(commandRunner, procFs);
        var options = new PortQueryOptions(ProtocolType.Udp);

        // act
        var results = await resolver.FindAsync(5000, options);

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    [DataRow("users:((\"dotnet\",pid=1234,fd=3))", 1234)]
    [DataRow("users:((\"dotnet\",pid=1234))", 1234)]
    [DataRow("nopidhere", 0)]
    [DataRow("pid=notanumber,fd=3", 0)]
    [DataRow("pid=999", 999)]
    public void ExtractPidFromSs_WithVariousFields_ReturnsExpectedPid(string field, int expected)
    {
        // act
        var result = LinuxPortResolver.ExtractPidFromSs(field);

        // assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [DataRow("users:((\"dotnet\",pid=1234,fd=3))", "dotnet")]
    [DataRow("noquotemarker", "<unknown>")]
    [DataRow("((\"\"", "<unknown>")]
    public void ExtractProcessNameFromSs_WithVariousFields_ReturnsExpectedName(string field, string expected)
    {
        // act
        var result = LinuxPortResolver.ExtractProcessNameFromSs(field);

        // assert
        Assert.AreEqual(expected, result);
    }
}
