using D20Tek.Tools.DevLog;
using D20Tek.Tools.UnitTests.DevLog.Fakes;

namespace D20Tek.Tools.UnitTests.DevLog.Commands;

[TestClass]
public class AddEntryCommandTests
{
    [TestMethod]
    public void Execute_WithValidProject_NoAccomplishments_ReturnsSuccess()
    {
        // arrange
        var fileAdapter = new FakeFileSystemAdapter();
        var context = TestContextFactory.CreateWithFakeFileAdapter(fileAdapter);
        context.Console.TestInput.PushTextWithEnter(string.Empty);

        // act
        var result = context.Run(["add", "MyProject"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains(Constants.AddEntrySuccess, result.Output);
    }

    [TestMethod]
    public void Execute_WithAccomplishments_AddsEntry()
    {
        // arrange
        var fileAdapter = new FakeFileSystemAdapter();
        var context = TestContextFactory.CreateWithFakeFileAdapter(fileAdapter);
        context.Console.TestInput.PushTextWithEnter("Implemented the feature");
        context.Console.TestInput.PushTextWithEnter(string.Empty);

        // act
        var result = context.Run(["add", "MyProject"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains(Constants.AddEntrySuccess, result.Output);
        Assert.Contains("- Implemented the feature", fileAdapter.LastWrittenContent);
    }

    [TestMethod]
    public void Execute_WithDateOption_ReturnsSuccess()
    {
        // arrange
        var fileAdapter = new FakeFileSystemAdapter();
        var context = TestContextFactory.CreateWithFakeFileAdapter(fileAdapter);
        context.Console.TestInput.PushTextWithEnter(string.Empty);

        // act
        var result = context.Run(["add", "MyProject", "-d", "01-08-2025"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains(Constants.AddEntrySuccess, result.Output);
    }

    [TestMethod]
    public void Execute_WithEmptyProjectName_ReturnsError()
    {
        // arrange
        var fileAdapter = new FakeFileSystemAdapter();
        var context = TestContextFactory.CreateWithFakeFileAdapter(fileAdapter);

        // act
        var result = context.Run(["add", ""]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(-1, result.ExitCode);
        Assert.Contains("Error:", result.Output);
    }
}
