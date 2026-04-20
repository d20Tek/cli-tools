using D20Tek.Tools.DevKillPort.Contracts;
using D20Tek.Tools.DevKillPort.Services;

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
}
