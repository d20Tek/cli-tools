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
    public void FindPidByInode_WithNonIntegerPidDirectory_SkipsDirectory()
    {
        // arrange
        var procFs = new FakeProcFileSystem()
            .WithPidDirectory("/proc/notanumber")
            .WithFdLink("/proc/notanumber", "/proc/notanumber/fd/3", "socket:[12345]");

        var scanner = new ProcFdScanner(procFs);

        // act
        var result = scanner.FindPidByInode(12345);

        // assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void FindPidByInode_WhenExistsThrows_ReturnsUnknownProcessName()
    {
        // arrange
        var procFs = new FakeProcFileSystem()
            .WithPidDirectory("/proc/1234")
            .WithThrowingExists("/proc/1234/comm")
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
    public void FindPidByInode_WhenReadAllLinesThrows_ReturnsUnknownProcessName()
    {
        // arrange
        var procFs = new FakeProcFileSystem()
            .WithPidDirectory("/proc/1234")
            .WithThrowingReadAllLines("/proc/1234/comm")
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
    public void FindPidByInode_WithWhitespaceOnlyComm_ReturnsUnknownProcessName()
    {
        // arrange
        var procFs = new FakeProcFileSystem()
            .WithPidDirectory("/proc/1234")
            .WithFile("/proc/1234/comm", ["   "])
            .WithFdLink("/proc/1234", "/proc/1234/fd/3", "socket:[12345]");

        var scanner = new ProcFdScanner(procFs);

        // act
        var result = scanner.FindPidByInode(12345);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1234, result.Pid);
        Assert.AreEqual("<unknown>", result.ProcessName);
    }
}

[TestClass]
public class PidInfoTests
{
    [TestMethod]
    public void Constructor_SetsProperties()
    {
        // act
        var pidInfo = new PidInfo(42, "dotnet");

        // assert
        Assert.AreEqual(42, pidInfo.Pid);
        Assert.AreEqual("dotnet", pidInfo.ProcessName);
    }

    [TestMethod]
    public void With_ChangesAllProperties_ReturnsNewInstance()
    {
        // arrange
        var original = new PidInfo(1, "original");

        // act
        var updated = original with { Pid = 99, ProcessName = "updated" };

        // assert
        Assert.AreNotSame(original, updated);
        Assert.AreEqual(99, updated.Pid);
        Assert.AreEqual("updated", updated.ProcessName);
    }

    [TestMethod]
    public void Equality_SameValues_AreEqual()
    {
        // arrange
        var a = new PidInfo(10, "proc");
        var b = new PidInfo(10, "proc");

        // assert
        Assert.AreEqual(a, b);
    }

    [TestMethod]
    public void Equality_DifferentValues_AreNotEqual()
    {
        // arrange
        var a = new PidInfo(10, "proc");
        var b = new PidInfo(20, "other");

        // assert
        Assert.AreNotEqual(a, b);
    }
}
