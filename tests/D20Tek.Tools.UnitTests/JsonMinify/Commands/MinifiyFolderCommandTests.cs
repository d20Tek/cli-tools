using D20Tek.Tools.UnitTests.JsonMinify.Fakes;
using System;
using System.Collections.Generic;
using System.Text;

namespace D20Tek.Tools.UnitTests.JsonMinify.Commands;

[TestClass]
public class MinifiyFolderCommandTests
{
    [TestMethod]
    public void Execute_ShouldReturnSuccess_WhenFolderPathIsValid()
    {
        // arrange
        var fileAdapter = new FakeFileSystemAdapter("{ \"name\": \"value\"}", ["file1.json", "file2.json"]);
        var context = TestContextFactory.CreateWithFakeFileAdapter(fileAdapter);

        // act
        var result = context.Run(["folder", "./test/folder"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains($"Folder with 2 files successfully minified.", result.Output);
    }

    [TestMethod]
    public void Execute_ShouldReturnError_WhenFolderPathIsInvalid()
    {
        // arrange
        var fileAdapter = new FakeFileSystemAdapter(string.Empty, null);
        var context = TestContextFactory.CreateWithFakeFileAdapter(fileAdapter);

        // act
        var result = context.Run(["folder", "./invalid/folder"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(-1, result.ExitCode);
        Assert.Contains("Error: The folder './invalid/folder' does not exist.", result.Output);
    }
}
