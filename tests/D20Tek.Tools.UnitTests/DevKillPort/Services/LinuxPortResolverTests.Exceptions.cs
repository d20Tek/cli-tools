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
}
