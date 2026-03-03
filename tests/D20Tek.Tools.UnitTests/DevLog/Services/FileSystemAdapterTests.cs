using D20Tek.Tools.Common.Services;

namespace D20Tek.Tools.UnitTests.DevLog.Services;

[TestClass]
public class FileSystemAdapterTests
{
    private string _testFolder = string.Empty;

    [TestInitialize]
    public void SetUp()
    {
        _testFolder = Path.Combine(Path.GetTempPath(), "devlog-fs-tests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testFolder);
    }

    [TestCleanup]
    public void TearDown()
    {
        if (Directory.Exists(_testFolder))
            Directory.Delete(_testFolder, true);
    }

    [TestMethod]
    public void Exists_WithExistingFile_ReturnsTrue()
    {
        // arrange
        var path = Path.Combine(_testFolder, "test.md");
        File.WriteAllText(path, "content");
        var adapter = new FileSystemAdapter();

        // act
        var result = adapter.Exists(path);

        // assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Exists_WithMissingFile_ReturnsFalse()
    {
        // arrange
        var path = Path.Combine(_testFolder, "missing.md");
        var adapter = new FileSystemAdapter();

        // act
        var result = adapter.Exists(path);

        // assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void FolderExists_WithExistingFolder_ReturnsTrue()
    {
        // arrange
        var adapter = new FileSystemAdapter();

        // act
        var result = adapter.FolderExists(_testFolder);

        // assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void FolderExists_WithMissingFolder_ReturnsFalse()
    {
        // arrange
        var path = Path.Combine(_testFolder, "nonexistent");
        var adapter = new FileSystemAdapter();

        // act
        var result = adapter.FolderExists(path);

        // assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void EnsureFolderExists_WithExistingFolder_ReturnsTrue()
    {
        // arrange
        var adapter = new FileSystemAdapter();

        // act
        var result = adapter.EnsureFolderExists(_testFolder);

        // assert
        Assert.IsTrue(result);
        Assert.IsTrue(Directory.Exists(_testFolder));
    }

    [TestMethod]
    public void EnsureFolderExists_WithNewFolder_CreatesFolderAndReturnsTrue()
    {
        // arrange
        var newFolder = Path.Combine(_testFolder, "new-subfolder");
        var adapter = new FileSystemAdapter();

        // act
        var result = adapter.EnsureFolderExists(newFolder);

        // assert
        Assert.IsTrue(result);
        Assert.IsTrue(Directory.Exists(newFolder));
    }

    [TestMethod]
    public void WriteAllText_WithValidPath_WritesContent()
    {
        // arrange
        var path = Path.Combine(_testFolder, "output.md");
        var content = "## Week of January 5, 2025\n\n### MyProject\n- Item 1";
        var adapter = new FileSystemAdapter();

        // act
        adapter.WriteAllText(path, content);

        // assert
        Assert.IsTrue(File.Exists(path));
        Assert.AreEqual(content, File.ReadAllText(path));
    }

    [TestMethod]
    public void ReadAllText_WithExistingFile_ReturnsContent()
    {
        // arrange
        var path = Path.Combine(_testFolder, "test.md");
        var content = "## Week of January 5, 2025\n\n### MyProject\n- Item 1";
        File.WriteAllText(path, content);
        var adapter = new FileSystemAdapter();

        // act
        var result = adapter.ReadAllText(path);

        // assert
        Assert.AreEqual(content, result);
    }

    [TestMethod]
    public void EnumerateFolderFiles_WithMatchingFiles_ReturnsMatchingCount()
    {
        // arrange
        File.WriteAllText(Path.Combine(_testFolder, "dev-log-20250105.md"), "content");
        File.WriteAllText(Path.Combine(_testFolder, "dev-log-20250112.md"), "content");
        File.WriteAllText(Path.Combine(_testFolder, "notes.txt"), "content");
        var adapter = new FileSystemAdapter();

        // act
        var result = adapter.EnumerateFolderFiles(_testFolder, "*.md").ToList();

        // assert
        Assert.HasCount(2, result);
    }

    [TestMethod]
    public void EnumerateFolderFiles_WithNoMatchingFiles_ReturnsEmpty()
    {
        // arrange
        File.WriteAllText(Path.Combine(_testFolder, "notes.txt"), "content");
        var adapter = new FileSystemAdapter();

        // act
        var result = adapter.EnumerateFolderFiles(_testFolder, "*.md").ToList();

        // assert
        Assert.HasCount(0, result);
    }
}
