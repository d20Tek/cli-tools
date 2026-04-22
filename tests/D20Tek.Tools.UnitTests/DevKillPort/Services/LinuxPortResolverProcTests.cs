using D20Tek.Tools.DevKillPort.Contracts;
using D20Tek.Tools.DevKillPort.Services;
using D20Tek.Tools.UnitTests.DevKillPort.Fakes;

namespace D20Tek.Tools.UnitTests.DevKillPort.Services;

[TestClass]
public class LinuxPortResolverProcTests
{
    [TestMethod]
    public void FindWithProc_WithMatchingSocketAndPid_ReturnsResult()
    {
        // arrange
        var commandRunner = new FakeCommandRunner();
        var procFs = new FakeProcFileSystem()
            .WithFile("/proc/net/tcp",
            [
                "  sl  local_address rem_address   st tx_queue rx_queue tr tm->when retrnsmt   uid  timeout inode",
                "   0: 00000000:1388 00000000:0000 0A 00000000:00000000 00:00000000 00000000     0        0 12345 1 0000000000000000 100 0 0 10 0"
            ])
            .WithPidDirectory("/proc/1234")
            .WithFile("/proc/1234/comm", ["dotnet"])
            .WithFdLink("/proc/1234", "/proc/1234/fd/3", "socket:[12345]");

        var resolver = new LinuxPortResolver(commandRunner, procFs);
        var options = new PortQueryOptions(ProtocolType.Tcp);

        // act
        var results = resolver.FindWithProc(5000, options);

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual(5000, results[0].Port);
        Assert.AreEqual(1234, results[0].ProcessId);
        Assert.AreEqual("dotnet", results[0].ProcessName);
        Assert.AreEqual("TCP", results[0].Protocol);
        Assert.AreEqual(PortState.Listen, results[0].State);
    }

    [TestMethod]
    public void FindWithProc_WithNoMatchingSockets_ReturnsEmpty()
    {
        // arrange
        var commandRunner = new FakeCommandRunner();
        var procFs = new FakeProcFileSystem()
            .WithFile("/proc/net/tcp",
            [
                "  sl  local_address rem_address   st tx_queue rx_queue tr tm->when retrnsmt   uid  timeout inode",
                "   0: 00000000:0050 00000000:0000 0A 00000000:00000000 00:00000000 00000000     0        0 99999 1 0000000000000000 100 0 0 10 0"
            ]);

        var resolver = new LinuxPortResolver(commandRunner, procFs);
        var options = new PortQueryOptions(ProtocolType.Tcp);

        // act
        var results = resolver.FindWithProc(5000, options);

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void FindWithProc_WithSocketButNoPidMatch_ReturnsEmpty()
    {
        // arrange
        var commandRunner = new FakeCommandRunner();
        var procFs = new FakeProcFileSystem()
            .WithFile("/proc/net/tcp",
            [
                "  sl  local_address rem_address   st tx_queue rx_queue tr tm->when retrnsmt   uid  timeout inode",
                "   0: 00000000:1388 00000000:0000 0A 00000000:00000000 00:00000000 00000000     0        0 12345 1 0000000000000000 100 0 0 10 0"
            ])
            .WithPidDirectory("/proc/1234")
            .WithFile("/proc/1234/comm", ["dotnet"])
            .WithFdLink("/proc/1234", "/proc/1234/fd/3", "socket:[99999]");

        var resolver = new LinuxPortResolver(commandRunner, procFs);
        var options = new PortQueryOptions(ProtocolType.Tcp);

        // act
        var results = resolver.FindWithProc(5000, options);

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void FindWithProc_DeduplicatesByPidAndProtocol()
    {
        // arrange
        var commandRunner = new FakeCommandRunner();
        var procFs = new FakeProcFileSystem()
            .WithFile("/proc/net/tcp",
            [
                "  sl  local_address rem_address   st tx_queue rx_queue tr tm->when retrnsmt   uid  timeout inode",
                "   0: 00000000:1388 00000000:0000 0A 00000000:00000000 00:00000000 00000000     0        0 12345 1 0000000000000000 100 0 0 10 0"
            ])
            .WithFile("/proc/net/tcp6",
            [
                "  sl  local_address rem_address   st tx_queue rx_queue tr tm->when retrnsmt   uid  timeout inode",
                "   0: 00000000000000000000000000000000:1388 00000000000000000000000000000000:0000 0A 00000000:00000000 00:00000000 00000000     0        0 12346 1 0000000000000000 100 0 0 10 0"
            ])
            .WithPidDirectory("/proc/1234")
            .WithFile("/proc/1234/comm", ["dotnet"])
            .WithFdLink("/proc/1234", "/proc/1234/fd/3", "socket:[12345]")
            .WithFdLink("/proc/1234", "/proc/1234/fd/4", "socket:[12346]");

        var resolver = new LinuxPortResolver(commandRunner, procFs);
        var options = new PortQueryOptions(ProtocolType.Tcp);

        // act
        var results = resolver.FindWithProc(5000, options);

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual(1234, results[0].ProcessId);
    }

    [TestMethod]
    public void FindWithProc_WithNoProcFiles_ReturnsEmpty()
    {
        // arrange
        var commandRunner = new FakeCommandRunner();
        var procFs = new FakeProcFileSystem();
        var resolver = new LinuxPortResolver(commandRunner, procFs);
        var options = new PortQueryOptions(ProtocolType.Both);

        // act
        var results = resolver.FindWithProc(5000, options);

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void ListAllWithProc_WithMatchingSocketAndPid_ReturnsResult()
    {
        // arrange
        var commandRunner = new FakeCommandRunner();
        var procFs = new FakeProcFileSystem()
            .WithFile("/proc/net/tcp",
            [
                "  sl  local_address rem_address   st tx_queue rx_queue tr tm->when retrnsmt   uid  timeout inode",
                "   0: 00000000:1388 00000000:0000 0A 00000000:00000000 00:00000000 00000000     0        0 12345 1 0000000000000000 100 0 0 10 0"
            ])
            .WithPidDirectory("/proc/1234")
            .WithFile("/proc/1234/comm", ["dotnet"])
            .WithFdLink("/proc/1234", "/proc/1234/fd/3", "socket:[12345]");
        var resolver = new LinuxPortResolver(commandRunner, procFs);
        var options = new PortQueryOptions(ProtocolType.Tcp);

        // act
        var results = resolver.ListAllWithProc(options);

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual(5000, results[0].Port);
        Assert.AreEqual(1234, results[0].ProcessId);
        Assert.AreEqual("dotnet", results[0].ProcessName);
        Assert.AreEqual("TCP", results[0].Protocol);
        Assert.AreEqual(PortState.Listen, results[0].State);
    }

    [TestMethod]
    public void ListAllWithProc_WithNoMatchingSockets_ReturnsEmpty()
    {
        // arrange
        var commandRunner = new FakeCommandRunner();
        var procFs = new FakeProcFileSystem();
        var resolver = new LinuxPortResolver(commandRunner, procFs);
        var options = new PortQueryOptions(ProtocolType.Tcp);

        // act
        var results = resolver.ListAllWithProc(options);

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void ListAllWithProc_WithSocketButNoPidMatch_ReturnsEmpty()
    {
        // arrange
        var commandRunner = new FakeCommandRunner();
        var procFs = new FakeProcFileSystem()
            .WithFile("/proc/net/tcp",
            [
                "  sl  local_address rem_address   st tx_queue rx_queue tr tm->when retrnsmt   uid  timeout inode",
                "   0: 00000000:1388 00000000:0000 0A 00000000:00000000 00:00000000 00000000     0        0 12345 1 0000000000000000 100 0 0 10 0"
            ])
            .WithPidDirectory("/proc/1234")
            .WithFile("/proc/1234/comm", ["dotnet"])
            .WithFdLink("/proc/1234", "/proc/1234/fd/3", "socket:[99999]");
        var resolver = new LinuxPortResolver(commandRunner, procFs);
        var options = new PortQueryOptions(ProtocolType.Tcp);

        // act
        var results = resolver.ListAllWithProc(options);

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void ListAllWithProc_DeduplicatesByPidProtocolAndPort()
    {
        // arrange
        var commandRunner = new FakeCommandRunner();
        var procFs = new FakeProcFileSystem()
            .WithFile("/proc/net/tcp",
            [
                "  sl  local_address rem_address   st tx_queue rx_queue tr tm->when retrnsmt   uid  timeout inode",
                "   0: 00000000:1388 00000000:0000 0A 00000000:00000000 00:00000000 00000000     0        0 12345 1 0000000000000000 100 0 0 10 0"
            ])
            .WithFile("/proc/net/tcp6",
            [
                "  sl  local_address rem_address   st tx_queue rx_queue tr tm->when retrnsmt   uid  timeout inode",
                "   0: 00000000000000000000000000000000:1388 00000000000000000000000000000000:0000 0A 00000000:00000000 00:00000000 00000000     0        0 12346 1 0000000000000000 100 0 0 10 0"
            ])
            .WithPidDirectory("/proc/1234")
            .WithFile("/proc/1234/comm", ["dotnet"])
            .WithFdLink("/proc/1234", "/proc/1234/fd/3", "socket:[12345]")
            .WithFdLink("/proc/1234", "/proc/1234/fd/4", "socket:[12346]");
        var resolver = new LinuxPortResolver(commandRunner, procFs);
        var options = new PortQueryOptions(ProtocolType.Tcp);

        // act
        var results = resolver.ListAllWithProc(options);

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual(1234, results[0].ProcessId);
        Assert.AreEqual(5000, results[0].Port);
    }
}
