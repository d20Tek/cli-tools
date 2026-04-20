using D20Tek.Tools.DevKillPort.Services;
using D20Tek.Tools.UnitTests.DevKillPort.Fakes;

namespace D20Tek.Tools.UnitTests.DevKillPort.Services;

[TestClass]
public class ProcFdScannerTests
{
    [TestMethod]
    public void FindPidByInode_WithMatchingInode_ReturnsPidInfo()
    {
        // arrange
        var procFs = new FakeProcFileSystem()
            .WithPidDirectory("/proc/1234")
            .WithFile("/proc/1234/comm", ["dotnet"])
            .WithFdLink("/proc/1234", "/proc/1234/fd/3", "socket:[12345]");

        var scanner = new ProcFdScanner(procFs);

        // act
        var result = scanner.FindPidByInode(12345);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1234, result.Pid);
        Assert.AreEqual("dotnet", result.ProcessName);
    }

    [TestMethod]
    public void FindPidByInode_WithNoMatchingInode_ReturnsNull()
    {
        // arrange
        var procFs = new FakeProcFileSystem()
            .WithPidDirectory("/proc/1234")
            .WithFile("/proc/1234/comm", ["dotnet"])
            .WithFdLink("/proc/1234", "/proc/1234/fd/3", "socket:[99999]");

        var scanner = new ProcFdScanner(procFs);

        // act
        var result = scanner.FindPidByInode(12345);

        // assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void FindPidByInode_WithNoPidDirectories_ReturnsNull()
    {
        // arrange
        var procFs = new FakeProcFileSystem();
        var scanner = new ProcFdScanner(procFs);

        // act
        var result = scanner.FindPidByInode(12345);

        // assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void FindPidByInode_CachesResults_OnSubsequentCalls()
    {
        // arrange
        var procFs = new FakeProcFileSystem()
            .WithPidDirectory("/proc/1234")
            .WithFile("/proc/1234/comm", ["dotnet"])
            .WithFdLink("/proc/1234", "/proc/1234/fd/3", "socket:[12345]");

        var scanner = new ProcFdScanner(procFs);

        // act
        var result1 = scanner.FindPidByInode(12345);
        var result2 = scanner.FindPidByInode(12345);

        // assert
        Assert.IsNotNull(result1);
        Assert.IsNotNull(result2);
        Assert.AreSame(result1, result2);
    }

    [TestMethod]
    public void InvalidateCache_ClearsCache_AllowsRefresh()
    {
        // arrange
        var procFs = new FakeProcFileSystem()
            .WithPidDirectory("/proc/1234")
            .WithFile("/proc/1234/comm", ["dotnet"])
            .WithFdLink("/proc/1234", "/proc/1234/fd/3", "socket:[12345]");

        var scanner = new ProcFdScanner(procFs);
        var result1 = scanner.FindPidByInode(12345);

        // act
        scanner.InvalidateCache();
        var result2 = scanner.FindPidByInode(12345);

        // assert
        Assert.IsNotNull(result1);
        Assert.IsNotNull(result2);
        Assert.AreNotSame(result1, result2);
    }

    [TestMethod]
    public void FindPidByInode_WithMissingComm_ReturnsUnknownProcessName()
    {
        // arrange
        var procFs = new FakeProcFileSystem()
            .WithPidDirectory("/proc/1234")
            .WithFdLink("/proc/1234", "/proc/1234/fd/3", "socket:[12345]");

        var scanner = new ProcFdScanner(procFs);

        // act
        var result = scanner.FindPidByInode(12345);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1234, result.Pid);
        Assert.AreEqual("<unknown>", result.ProcessName);
    }

    [TestMethod]
    public void FindPidByInode_WithNonSocketLink_SkipsLink()
    {
        // arrange
        var procFs = new FakeProcFileSystem()
            .WithPidDirectory("/proc/1234")
            .WithFile("/proc/1234/comm", ["dotnet"])
            .WithFdLink("/proc/1234", "/proc/1234/fd/3", "/dev/null");

        var scanner = new ProcFdScanner(procFs);

        // act
        var result = scanner.FindPidByInode(12345);

        // assert
        Assert.IsNull(result);
    }

    [TestMethod]
    [DataRow("socket:[12345]", true, 12345L)]
    [DataRow("socket:[0]", false, 0L)]
    [DataRow("/dev/null", false, 0L)]
    [DataRow("socket:[]", false, 0L)]
    [DataRow("pipe:[999]", false, 0L)]
    [DataRow("socket:[abc]", false, 0L)]
    public void TryExtractSocketInode_WithVariousInputs_ReturnsExpected(
        string input, bool expectedResult, long expectedInode)
    {
        // act
        var result = ProcFdScanner.TryExtractSocketInode(input, out var inode);

        // assert
        Assert.AreEqual(expectedResult, result);
        if (expectedResult)
        {
            Assert.AreEqual(expectedInode, inode);
        }
    }

    [TestMethod]
    public void FindPidByInode_WithMultiplePids_FindsCorrectOne()
    {
        // arrange
        var procFs = new FakeProcFileSystem()
            .WithPidDirectory("/proc/100")
            .WithFile("/proc/100/comm", ["nginx"])
            .WithFdLink("/proc/100", "/proc/100/fd/3", "socket:[11111]")
            .WithPidDirectory("/proc/200")
            .WithFile("/proc/200/comm", ["dotnet"])
            .WithFdLink("/proc/200", "/proc/200/fd/5", "socket:[22222]");

        var scanner = new ProcFdScanner(procFs);

        // act
        var result = scanner.FindPidByInode(22222);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.Pid);
        Assert.AreEqual("dotnet", result.ProcessName);
    }
}
