using D20Tek.Tools.DevKillPort.Contracts;
using D20Tek.Tools.DevKillPort.Services;
using D20Tek.Tools.UnitTests.DevKillPort.Fakes;

namespace D20Tek.Tools.UnitTests.DevKillPort.Services;

[TestClass]
public class LinuxPortResolverExceptionTests
{
    [TestMethod]
    public async Task FindAsync_WhenSsThrows_ReturnsEmptyAndFallsThrough()
    {
        // arrange
        var commandRunner = new ThrowingCommandRunner();
        var procFs = new ThrowingProcFileSystem();
        var resolver = new LinuxPortResolver(commandRunner, procFs);
        var options = new PortQueryOptions(ProtocolType.Both);

        // act
        var results = await resolver.FindAsync(5000, options);

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public async Task FindAsync_WhenSsThrowsAndLsofThrows_FallsBackToProc()
    {
        // arrange
        var commandRunner = new ThrowingCommandRunner();
        var procFs = new FakeProcFileSystem();
        var resolver = new LinuxPortResolver(commandRunner, procFs);
        var options = new PortQueryOptions(ProtocolType.Tcp);

        // act
        var results = await resolver.FindAsync(5000, options);

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public void FindWithProc_WhenProcFsThrows_ReturnsEmptyList()
    {
        // arrange
        var commandRunner = new FakeCommandRunner();
        var procFs = new ThrowingProcFileSystem();
        var resolver = new LinuxPortResolver(commandRunner, procFs);
        var options = new PortQueryOptions(ProtocolType.Tcp);

        // act
        var results = resolver.FindWithProc(5000, options);

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public async Task ListAllAsync_WhenSsThrows_FallsBackToProc()
    {
        // arrange
        var commandRunner = new ThrowingCommandRunner();
        var procFs = new FakeProcFileSystem();
        var resolver = new LinuxPortResolver(commandRunner, procFs);
        var options = new PortQueryOptions(ProtocolType.Both);

        // act
        var results = await resolver.ListAllAsync(options);

        // assert
        Assert.HasCount(0, results);
    }

    [TestMethod]
    public async Task ListAllAsync_WhenSsThrowsAndLsofThrows_FallsBackToProc()
    {
        // arrange
        var commandRunner = new ThrowingCommandRunner();
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
        var options = new PortQueryOptions(ProtocolType.Tcp);

        // act
        var results = await resolver.ListAllAsync(options);

        // assert
        Assert.HasCount(1, results);
        Assert.AreEqual(9999, results[0].ProcessId);
        Assert.AreEqual(5000, results[0].Port);
    }

    [TestMethod]
    public void ListAllWithProc_WhenProcFsThrows_ReturnsEmptyList()
    {
        // arrange
        var commandRunner = new FakeCommandRunner();
        var procFs = new ThrowingProcFileSystem();
        var resolver = new LinuxPortResolver(commandRunner, procFs);
        var options = new PortQueryOptions(ProtocolType.Tcp);

        // act
        var results = resolver.ListAllWithProc(options);

        // assert
        Assert.HasCount(0, results);
    }
}
