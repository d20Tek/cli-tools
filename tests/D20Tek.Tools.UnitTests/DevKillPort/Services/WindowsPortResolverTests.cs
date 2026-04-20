using D20Tek.Tools.DevKillPort.Contracts;
using D20Tek.Tools.DevKillPort.Services;

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
}
