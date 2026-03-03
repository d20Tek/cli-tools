using D20Tek.Tools.DevLog;
using D20Tek.Tools.UnitTests.DevLog.Fakes;

namespace D20Tek.Tools.UnitTests.DevLog.Commands;

[TestClass]
public class ListLogsCommandTests
{
    [TestMethod]
    public void Execute_WithFiles_DisplaysFileList()
    {
        // arrange
        var files = new[] { "dev-log-20250105.md", "dev-log-20250112.md" };
        var fileAdapter = new FakeFileSystemAdapter(folderFiles: files);
        var context = TestContextFactory.CreateWithFakeFileAdapter(fileAdapter);

        // act
        var result = context.Run(["list"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains(Constants.ListLogsSucceeded, result.Output);
        Assert.Contains("dev-log-20250105.md", result.Output);
        Assert.Contains("dev-log-20250112.md", result.Output);
    }

    [TestMethod]
    public void Execute_WithFiles_DisplaysWeekOfDates()
    {
        // arrange
        var files = new[] { "dev-log-20250105.md" };
        var fileAdapter = new FakeFileSystemAdapter(folderFiles: files);
        var context = TestContextFactory.CreateWithFakeFileAdapter(fileAdapter);

        // act
        var result = context.Run(["list"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains("January 5, 2025", result.Output);
    }

    [TestMethod]
    public void Execute_WithEmptyFolder_ShowsEmptyMessage()
    {
        // arrange
        var fileAdapter = new FakeFileSystemAdapter(folderFiles: []);
        var context = TestContextFactory.CreateWithFakeFileAdapter(fileAdapter);

        // act
        var result = context.Run(["list"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains("No dev-log files found", result.Output);
    }

    [TestMethod]
    public void Execute_WithFolderOption_UsesSpecifiedFolder()
    {
        // arrange
        var files = new[] { "dev-log-20250105.md" };
        var fileAdapter = new FakeFileSystemAdapter(folderFiles: files);
        var context = TestContextFactory.CreateWithFakeFileAdapter(fileAdapter);

        // act
        var result = context.Run(["list", "-f", "./my-logs"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains(Constants.ListLogsSucceeded, result.Output);
    }

    [TestMethod]
    public void Execute_WithNonStandardFileName_DisplaysWithoutDate()
    {
        // arrange
        var files = new[] { "notes.md" };
        var fileAdapter = new FakeFileSystemAdapter(folderFiles: files);
        var context = TestContextFactory.CreateWithFakeFileAdapter(fileAdapter);

        // act
        var result = context.Run(["list"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains("notes.md", result.Output);
    }
}
