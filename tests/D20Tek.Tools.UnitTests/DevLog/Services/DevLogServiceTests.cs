using D20Tek.Tools.DevLog.Services;
using D20Tek.Tools.UnitTests.DevLog.Fakes;

namespace D20Tek.Tools.UnitTests.DevLog.Services;

[TestClass]
public class DevLogServiceTests
{
    private static readonly string _logFolder = "./devlogs";
    private static readonly DateOnly _testDate = new(2025, 1, 8);
    private static readonly string _existingContent =
        "## Week of January 5, 2025\n\n### MyProject\n- Item 1\n- Item 2";

    [TestMethod]
    public void AddEntry_WithNewFile_CreatesFileAndReturnsSuccess()
    {
        // arrange
        var fileAdapter = new FakeFileSystemAdapter();
        var service = new DevLogService(fileAdapter);

        // act
        var result = service.AddEntry(_logFolder, "MyProject", ["Item 1"], _testDate);

        // assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(result.GetValue());
        Assert.Contains("### MyProject", fileAdapter.LastWrittenContent);
        Assert.Contains("- Item 1", fileAdapter.LastWrittenContent);
    }

    [TestMethod]
    public void AddEntry_WithExistingFile_SameProject_MergesAccomplishments()
    {
        // arrange
        var fileAdapter = new FakeFileSystemAdapter(_existingContent);
        var service = new DevLogService(fileAdapter);

        // act
        var result = service.AddEntry(_logFolder, "MyProject", ["Item 3"], _testDate);

        // assert
        Assert.IsTrue(result.IsSuccess);
        Assert.Contains("- Item 1", fileAdapter.LastWrittenContent);
        Assert.Contains("- Item 2", fileAdapter.LastWrittenContent);
        Assert.Contains("- Item 3", fileAdapter.LastWrittenContent);
    }

    [TestMethod]
    public void AddEntry_WithExistingFile_NewProject_AppendsProject()
    {
        // arrange
        var fileAdapter = new FakeFileSystemAdapter(_existingContent);
        var service = new DevLogService(fileAdapter);

        // act
        var result = service.AddEntry(_logFolder, "OtherProject", ["New item"], _testDate);

        // assert
        Assert.IsTrue(result.IsSuccess);
        Assert.Contains("### MyProject", fileAdapter.LastWrittenContent);
        Assert.Contains("### OtherProject", fileAdapter.LastWrittenContent);
        Assert.Contains("- New item", fileAdapter.LastWrittenContent);
    }

    [TestMethod]
    public void ViewLog_WithExistingFile_ReturnsContent()
    {
        // arrange
        var fileAdapter = new FakeFileSystemAdapter(_existingContent);
        var service = new DevLogService(fileAdapter);

        // act
        var result = service.ViewLog(_logFolder, _testDate);

        // assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(_existingContent, result.GetValue());
    }

    [TestMethod]
    public void ViewLog_WithMissingFile_ReturnsFailure()
    {
        // arrange
        var fileAdapter = new FakeFileSystemAdapter();
        var service = new DevLogService(fileAdapter);

        // act
        var result = service.ViewLog(_logFolder, _testDate);

        // assert
        Assert.IsFalse(result.IsSuccess);
    }

    [TestMethod]
    public void EditEntry_WithExistingFile_UpdatesEntry()
    {
        // arrange
        var fileAdapter = new FakeFileSystemAdapter(_existingContent);
        var service = new DevLogService(fileAdapter);

        // act
        var result = service.EditEntry(_logFolder, "MyProject", ["Updated item"], _testDate);

        // assert
        Assert.IsTrue(result.IsSuccess);
        Assert.Contains("- Updated item", fileAdapter.LastWrittenContent);
        Assert.DoesNotContain("- Item 1", fileAdapter.LastWrittenContent);
    }

    [TestMethod]
    public void EditEntry_WithMissingFile_ReturnsFailure()
    {
        // arrange
        var fileAdapter = new FakeFileSystemAdapter();
        var service = new DevLogService(fileAdapter);

        // act
        var result = service.EditEntry(_logFolder, "MyProject", ["Item 1"], _testDate);

        // assert
        Assert.IsFalse(result.IsSuccess);
    }

    [TestMethod]
    public void ListLogs_WithFiles_ReturnsFileList()
    {
        // arrange
        var files = new[] { "dev-log-20250105.md", "dev-log-20250112.md" };
        var fileAdapter = new FakeFileSystemAdapter(folderFiles: files);
        var service = new DevLogService(fileAdapter);

        // act
        var result = service.ListLogs(_logFolder);

        // assert
        Assert.IsTrue(result.IsSuccess);
        Assert.HasCount(2, result.GetValue().ToList());
    }

    [TestMethod]
    public void ListLogs_WithEmptyFolder_ReturnsEmptyList()
    {
        // arrange
        var fileAdapter = new FakeFileSystemAdapter(folderFiles: []);
        var service = new DevLogService(fileAdapter);

        // act
        var result = service.ListLogs(_logFolder);

        // assert
        Assert.IsTrue(result.IsSuccess);
        Assert.HasCount(0, result.GetValue().ToList());
    }

    [TestMethod]
    public void AddEntry_WithMultipleExistingProjects_MergesOnlyMatchingProject()
    {
        // arrange
        var content = "## Week of January 5, 2025\n\n### Project1\n- P1 Item 1\n\n### Project2\n- P2 Item 1";
        var fileAdapter = new FakeFileSystemAdapter(content);
        var service = new DevLogService(fileAdapter);

        // act
        var result = service.AddEntry(_logFolder, "Project1", ["P1 Item 2"], _testDate);

        // assert
        Assert.IsTrue(result.IsSuccess);
        Assert.Contains("- P1 Item 1", fileAdapter.LastWrittenContent);
        Assert.Contains("- P1 Item 2", fileAdapter.LastWrittenContent);
        Assert.Contains("- P2 Item 1", fileAdapter.LastWrittenContent);
    }

    [TestMethod]
    public void EditEntry_WithMultipleExistingProjects_ReplacesOnlyMatchingProject()
    {
        // arrange
        var content = "## Week of January 5, 2025\n\n### Project1\n- P1 Item 1\n\n### Project2\n- P2 Item 1";
        var fileAdapter = new FakeFileSystemAdapter(content);
        var service = new DevLogService(fileAdapter);

        // act
        var result = service.EditEntry(_logFolder, "Project1", ["P1 Updated"], _testDate);

        // assert
        Assert.IsTrue(result.IsSuccess);
        Assert.Contains("- P1 Updated", fileAdapter.LastWrittenContent);
        Assert.DoesNotContain("- P1 Item 1", fileAdapter.LastWrittenContent);
        Assert.Contains("- P2 Item 1", fileAdapter.LastWrittenContent);
    }

    [TestMethod]
    public void EditEntry_WithProjectNotInFile_AppendsNewProject()
    {
        // arrange
        var fileAdapter = new FakeFileSystemAdapter(_existingContent);
        var service = new DevLogService(fileAdapter);

        // act
        var result = service.EditEntry(_logFolder, "NewProject", ["New Item"], _testDate);

        // assert
        Assert.IsTrue(result.IsSuccess);
        Assert.Contains("### MyProject", fileAdapter.LastWrittenContent);
        Assert.Contains("### NewProject", fileAdapter.LastWrittenContent);
        Assert.Contains("- New Item", fileAdapter.LastWrittenContent);
    }
}
