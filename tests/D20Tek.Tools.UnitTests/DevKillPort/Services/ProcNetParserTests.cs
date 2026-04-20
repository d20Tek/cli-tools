using D20Tek.Tools.DevKillPort.Contracts;
using D20Tek.Tools.DevKillPort.Services;
using D20Tek.Tools.UnitTests.DevKillPort.Fakes;

namespace D20Tek.Tools.UnitTests.DevKillPort.Services;

[TestClass]
public class ProcNetParserTests
{
    [TestMethod]
    public void ParseProcNetFile_WithValidTcpEntry_ReturnsParsedSocket()
    {
        // arrange
        var procFs = new FakeProcFileSystem()
            .WithFile("/proc/net/tcp",
            [
                "  sl  local_address rem_address   st tx_queue rx_queue tr tm->when retrnsmt   uid  timeout inode",
                "   0: 00000000:1388 00000000:0000 0A 00000000:00000000 00:00000000 00000000     0        0 12345 1 0000000000000000 100 0 0 10 0"
            ]);

        var parser = new ProcNetParser(procFs);

        // act
        var results = parser.ParseProcNetFile("/proc/net/tcp", 5000, "TCP");

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual(5000, results[0].Port);
        Assert.AreEqual(12345L, results[0].Inode);
        Assert.AreEqual("TCP", results[0].Protocol);
        Assert.AreEqual(PortState.Listen, results[0].State);
        Assert.AreEqual("0.0.0.0", results[0].Address);
    }

    [TestMethod]
    public void ParseProcNetFile_WithDifferentPort_ReturnsEmpty()
    {
        // arrange
        var procFs = new FakeProcFileSystem()
            .WithFile("/proc/net/tcp",
            [
                "  sl  local_address rem_address   st tx_queue rx_queue tr tm->when retrnsmt   uid  timeout inode",
                "   0: 00000000:0050 00000000:0000 0A 00000000:00000000 00:00000000 00000000     0        0 99999 1 0000000000000000 100 0 0 10 0"
            ]);

        var parser = new ProcNetParser(procFs);

        // act
        var results = parser.ParseProcNetFile("/proc/net/tcp", 5000, "TCP");

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void ParseProcNetFile_WithMissingFile_ReturnsEmpty()
    {
        // arrange
        var procFs = new FakeProcFileSystem();
        var parser = new ProcNetParser(procFs);

        // act
        var results = parser.ParseProcNetFile("/proc/net/tcp", 5000, "TCP");

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void ParseProcNetFile_WithZeroInode_SkipsEntry()
    {
        // arrange
        var procFs = new FakeProcFileSystem()
            .WithFile("/proc/net/tcp",
            [
                "  sl  local_address rem_address   st tx_queue rx_queue tr tm->when retrnsmt   uid  timeout inode",
                "   0: 00000000:1388 00000000:0000 0A 00000000:00000000 00:00000000 00000000     0        0 0 1 0000000000000000 100 0 0 10 0"
            ]);

        var parser = new ProcNetParser(procFs);

        // act
        var results = parser.ParseProcNetFile("/proc/net/tcp", 5000, "TCP");

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void FindSocketsByPort_WithTcpProtocol_OnlyParseTcpFiles()
    {
        // arrange
        var procFs = new FakeProcFileSystem()
            .WithFile("/proc/net/tcp",
            [
                "  sl  local_address rem_address   st tx_queue rx_queue tr tm->when retrnsmt   uid  timeout inode",
                "   0: 00000000:1388 00000000:0000 0A 00000000:00000000 00:00000000 00000000     0        0 12345 1 0000000000000000 100 0 0 10 0"
            ]);

        var parser = new ProcNetParser(procFs);

        // act
        var results = parser.FindSocketsByPort(5000, ProtocolType.Tcp);

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual("TCP", results[0].Protocol);
    }

    [TestMethod]
    [DataRow("0A", 0)]
    [DataRow("01", 1)]
    [DataRow("06", 2)]
    [DataRow("08", 3)]
    [DataRow("FF", 4)]
    public void ParseTcpState_WithVariousHexStates_ReturnsCorrectEnum(string hexState, int expected)
    {
        // act
        var result = ProcNetParser.ParseTcpState(hexState);

        // assert
        Assert.AreEqual((PortState)expected, result);
    }

    [TestMethod]
    public void ParseLine_WithEstablishedState_ReturnsEstablished()
    {
        // arrange
        var line =
            "   0: 0100007F:1388 0100007F:CB20 01 00000000:00000000 00:00000000 00000000     0        0 54321 1 0000000000000000 100 0 0 10 0";

        // act
        var result = ProcNetParser.ParseLine(line, 5000, "TCP");

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(PortState.Established, result.State);
        Assert.AreEqual(54321L, result.Inode);
        Assert.AreEqual("127.0.0.1", result.Address);
    }

    [TestMethod]
    public void ParseLine_WithTooFewParts_ReturnsNull()
    {
        // arrange
        var line = "   0: 00000000:1388";

        // act
        var result = ProcNetParser.ParseLine(line, 5000, "TCP");

        // assert
        Assert.IsNull(result);
    }
}
