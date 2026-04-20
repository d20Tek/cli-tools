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

    [TestMethod]
    public void ParseLine_WithNoColonInLocalAddress_ReturnsNull()
    {
        // arrange - local address field has no colon
        var line = "   0: 000000001388 00000000:0000 0A 00000000:00000000 00:00000000 00000000     0        0 12345 1 0000000000000000 100 0 0 10 0";

        // act
        var result = ProcNetParser.ParseLine(line, 5000, "TCP");

        // assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void ParseLine_WithInvalidHexPort_ReturnsNull()
    {
        // arrange - port portion is not valid hex
        var line = "   0: 00000000:ZZZZ 00000000:0000 0A 00000000:00000000 00:00000000 00000000     0        0 12345 1 0000000000000000 100 0 0 10 0";

        // act
        var result = ProcNetParser.ParseLine(line, 5000, "TCP");

        // assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void ParseLine_WithNonIntegerInode_ReturnsNull()
    {
        // arrange - inode field is not a number
        var line = "   0: 00000000:1388 00000000:0000 0A 00000000:00000000 00:00000000 00000000     0        0 XXXX 1 0000000000000000 100 0 0 10 0";

        // act
        var result = ProcNetParser.ParseLine(line, 5000, "TCP");

        // assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void ParseLine_WithIPv6Address_ReturnsDoubleColonAddress()
    {
        // arrange - 32-char hex address (IPv6)
        var line = "   0: 00000000000000000000000000000000:1388 00000000000000000000000000000000:0000 0A 00000000:00000000 00:00000000 00000000     0        0 55555 1 0000000000000000 100 0 0 10 0";

        // act
        var result = ProcNetParser.ParseLine(line, 5000, "TCP");

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("[::]", result.Address);
    }

    [TestMethod]
    public void ParseLine_WithShortHexAddress_ReturnRawAddress()
    {
        // arrange - address that is neither 8 nor 32 chars (raw fallback)
        var line = "   0: 0000:1388 00000000:0000 0A 00000000:00000000 00:00000000 00000000     0        0 77777 1 0000000000000000 100 0 0 10 0";

        // act
        var result = ProcNetParser.ParseLine(line, 5000, "TCP");

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("0000", result.Address);
    }

    [TestMethod]
    public void FindSocketsByPort_WithUdpProtocol_OnlyParsesUdpFiles()
    {
        // arrange
        var procFs = new FakeProcFileSystem()
            .WithFile("/proc/net/udp",
            [
                "  sl  local_address rem_address   st tx_queue rx_queue tr tm->when retrnsmt   uid  timeout inode",
                "   0: 00000000:1388 00000000:0000 07 00000000:00000000 00:00000000 00000000     0        0 22222 1 0000000000000000 100 0 0 10 0"
            ]);

        var parser = new ProcNetParser(procFs);

        // act
        var results = parser.FindSocketsByPort(5000, ProtocolType.Udp);

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual("UDP", results[0].Protocol);
    }

    [TestMethod]
    public void FindSocketsByPort_WithBothProtocols_ParsesTcpAndUdpFiles()
    {
        // arrange
        var procFs = new FakeProcFileSystem()
            .WithFile("/proc/net/tcp",
            [
                "  sl  local_address rem_address   st tx_queue rx_queue tr tm->when retrnsmt   uid  timeout inode",
                "   0: 00000000:1388 00000000:0000 0A 00000000:00000000 00:00000000 00000000     0        0 11111 1 0000000000000000 100 0 0 10 0"
            ])
            .WithFile("/proc/net/udp",
            [
                "  sl  local_address rem_address   st tx_queue rx_queue tr tm->when retrnsmt   uid  timeout inode",
                "   0: 00000000:1388 00000000:0000 07 00000000:00000000 00:00000000 00000000     0        0 22222 1 0000000000000000 100 0 0 10 0"
            ]);

        var parser = new ProcNetParser(procFs);

        // act
        var results = parser.FindSocketsByPort(5000, ProtocolType.Both);

        // assert
        Assert.HasCount(2, results);
        Assert.IsTrue(results.Any(r => r.Protocol == "TCP"));
        Assert.IsTrue(results.Any(r => r.Protocol == "UDP"));
    }

    [TestMethod]
    public void TryParseHexPort_WithInvalidHex_ReturnsFalseAndZero()
    {
        // act
        var result = ProcNetParser.TryParseHexPort("ZZZZ", out var port);

        // assert
        Assert.IsFalse(result);
        Assert.AreEqual(0, port);
    }

    [TestMethod]
    public void TryParseHexPort_WithValidHex_ReturnsTrueAndPort()
    {
        // act
        var result = ProcNetParser.TryParseHexPort("1388", out var port);

        // assert
        Assert.IsTrue(result);
        Assert.AreEqual(5000, port);
    }
}

[TestClass]
public class ProcSocketEntryTests
{
    [TestMethod]
    public void Constructor_SetsProperties()
    {
        // act
        var entry = new ProcSocketEntry(8080, 99999L, "TCP", "127.0.0.1", PortState.Listen);

        // assert
        Assert.AreEqual(8080, entry.Port);
        Assert.AreEqual(99999L, entry.Inode);
        Assert.AreEqual("TCP", entry.Protocol);
        Assert.AreEqual("127.0.0.1", entry.Address);
        Assert.AreEqual(PortState.Listen, entry.State);
    }

    [TestMethod]
    public void With_ChangesAllProperties_ReturnsNewInstance()
    {
        // arrange
        var original = new ProcSocketEntry(8080, 99999L, "TCP", "127.0.0.1", PortState.Listen);

        // act
        var updated = original with { Port = 443, Inode = 11111L, Protocol = "UDP", Address = "0.0.0.0", State = PortState.Established };

        // assert
        Assert.AreNotSame(original, updated);
        Assert.AreEqual(443, updated.Port);
        Assert.AreEqual(11111L, updated.Inode);
        Assert.AreEqual("UDP", updated.Protocol);
        Assert.AreEqual("0.0.0.0", updated.Address);
        Assert.AreEqual(PortState.Established, updated.State);
    }

    [TestMethod]
    public void Equality_SameValues_AreEqual()
    {
        // arrange
        var a = new ProcSocketEntry(8080, 99999L, "TCP", "127.0.0.1", PortState.Listen);
        var b = new ProcSocketEntry(8080, 99999L, "TCP", "127.0.0.1", PortState.Listen);

        // assert
        Assert.AreEqual(a, b);
    }

    [TestMethod]
    public void Equality_DifferentValues_AreNotEqual()
    {
        // arrange
        var a = new ProcSocketEntry(8080, 99999L, "TCP", "127.0.0.1", PortState.Listen);
        var b = new ProcSocketEntry(443, 11111L, "UDP", "0.0.0.0", PortState.Established);

        // assert
        Assert.AreNotEqual(a, b);
    }
}
