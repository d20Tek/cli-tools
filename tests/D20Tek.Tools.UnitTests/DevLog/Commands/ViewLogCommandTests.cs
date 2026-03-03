using D20Tek.Tools.DevLog;
using D20Tek.Tools.UnitTests.DevLog.Fakes;

namespace D20Tek.Tools.UnitTests.DevLog.Commands;

[TestClass]
public class ViewLogCommandTests
{
    private const string _existingContent =
        "## Week of January 5, 2025\n\n### MyProject\n- Item 1\n- Item 2";

    [TestMethod]
    public void Execute_WithExistingFile_ReturnsContent()
    {
        // arrange
        var fileAdapter = new FakeFileSystemAdapter(_existingContent);
        var context = TestContextFactory.CreateWithFakeFileAdapter(fileAdapter);

        // act
        var result = context.Run(["view"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains(Constants.ViewLogSucceeded, result.Output);
    }

    [TestMethod]
    public void Execute_WithDateOption_ReturnsContent()
    {
        // arrange
        var fileAdapter = new FakeFileSystemAdapter(_existingContent);
        var context = TestContextFactory.CreateWithFakeFileAdapter(fileAdapter);

        // act
        var result = context.Run(["view", "-d", "01-08-2025"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains(Constants.ViewLogSucceeded, result.Output);
    }

    [TestMethod]
    public void Execute_WithMissingFile_ReturnsError()
    {
        // arrange
        var fileAdapter = new FakeFileSystemAdapter();
        var context = TestContextFactory.CreateWithFakeFileAdapter(fileAdapter);

        // act
        var result = context.Run(["view"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(-1, result.ExitCode);
        Assert.Contains("Error:", result.Output);
    }
}
