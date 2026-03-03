using D20Tek.Tools.DevLog;
using D20Tek.Tools.UnitTests.DevLog.Fakes;

namespace D20Tek.Tools.UnitTests.DevLog.Commands;

[TestClass]
public class EditEntryCommandTests
{
    private const string _existingContent = "## Week of January 5, 2025\n\n### MyProject\n- Item 1\n- Item 2";

    [TestMethod]
    public void Execute_WithExistingEntry_NoEdits_ReturnsSuccess()
    {
        // arrange
        var fileAdapter = new FakeFileSystemAdapter(_existingContent);
        var context = TestContextFactory.CreateWithFakeFileAdapter(fileAdapter);
        context.Console.TestInput.PushTextWithEnter(string.Empty);

        // act
        var result = context.Run(["edit", "MyProject"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains(Constants.EditEntrySuccess, result.Output);
    }

    [TestMethod]
    public void Execute_WithExistingEntry_EditsLine_ReturnsSuccess()
    {
        // arrange
        var fileAdapter = new FakeFileSystemAdapter(_existingContent);
        var context = TestContextFactory.CreateWithFakeFileAdapter(fileAdapter);
        context.Console.TestInput.PushTextWithEnter("1");
        context.Console.TestInput.PushTextWithEnter("Updated item 1");
        context.Console.TestInput.PushTextWithEnter(string.Empty);

        // act
        var result = context.Run(["edit", "MyProject"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains(Constants.EditEntrySuccess, result.Output);
        Assert.Contains("- Updated item 1", fileAdapter.LastWrittenContent);
    }

    [TestMethod]
    public void Execute_WithInvalidLineNumber_ShowsErrorAndContinues()
    {
        // arrange
        var fileAdapter = new FakeFileSystemAdapter(_existingContent);
        var context = TestContextFactory.CreateWithFakeFileAdapter(fileAdapter);
        context.Console.TestInput.PushTextWithEnter("99");
        context.Console.TestInput.PushTextWithEnter(string.Empty);

        // act
        var result = context.Run(["edit", "MyProject"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains("Invalid line number", result.Output);
    }

    [TestMethod]
    public void Execute_WithEmptyProjectName_ReturnsError()
    {
        // arrange
        var fileAdapter = new FakeFileSystemAdapter(_existingContent);
        var context = TestContextFactory.CreateWithFakeFileAdapter(fileAdapter);

        // act
        var result = context.Run(["edit", ""]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(-1, result.ExitCode);
        Assert.Contains("Error:", result.Output);
    }

    [TestMethod]
    public void Execute_WithMissingFile_ReturnsError()
    {
        // arrange
        var fileAdapter = new FakeFileSystemAdapter();
        var context = TestContextFactory.CreateWithFakeFileAdapter(fileAdapter);
        context.Console.TestInput.PushTextWithEnter(string.Empty);

        // act
        var result = context.Run(["edit", "MyProject"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(-1, result.ExitCode);
        Assert.Contains("Error:", result.Output);
    }

    [TestMethod]
    public void Execute_WithFolderAndDateOptions_ReturnsSuccess()
    {
        // arrange
        var fileAdapter = new FakeFileSystemAdapter(_existingContent);
        var context = TestContextFactory.CreateWithFakeFileAdapter(fileAdapter);
        context.Console.TestInput.PushTextWithEnter(string.Empty);

        // act
        var result = context.Run(["edit", "MyProject", "-f", "./logs", "-d", "01-08-2025"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains(Constants.EditEntrySuccess, result.Output);
    }

    [TestMethod]
    public void Execute_WithNonNumericLineInput_ShowsInvalidLineError()
    {
        // arrange
        var fileAdapter = new FakeFileSystemAdapter(_existingContent);
        var context = TestContextFactory.CreateWithFakeFileAdapter(fileAdapter);
        context.Console.TestInput.PushTextWithEnter("abc");
        context.Console.TestInput.PushTextWithEnter(string.Empty);

        // act
        var result = context.Run(["edit", "MyProject"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains("Invalid line number", result.Output);
    }

    [TestMethod]
    public void Execute_WithLineNumberBelowRange_ShowsInvalidLineError()
    {
        // arrange
        var fileAdapter = new FakeFileSystemAdapter(_existingContent);
        var context = TestContextFactory.CreateWithFakeFileAdapter(fileAdapter);
        context.Console.TestInput.PushTextWithEnter("0");
        context.Console.TestInput.PushTextWithEnter(string.Empty);

        // act
        var result = context.Run(["edit", "MyProject"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains("Invalid line number", result.Output);
    }
}

