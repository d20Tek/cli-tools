using D20Tek.Tools.UnitTests.JsonMinify.Fakes;

namespace D20Tek.Tools.UnitTests.JsonMinify.Commands;

[TestClass]
public class MinifyFileCommandTests
{
    [TestMethod]
    public void Execute_ShouldReturnSuccess_WhenFilePathIsValid()
    {
        // arrange
        var fileAdapter = new FakeFileSystemAdapter(
            @"{
                ""name"": ""John"",
                ""age"": 30,
                ""city"": ""New York""
            }");
        var context = TestContextFactory.CreateWithFakeFileAdapter(fileAdapter);

        // act
        var result = context.Run(["file", "./test/file.json"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains("Json file was successfully minified.", result.Output);
    }

    [TestMethod]
    public void Execute_WithTargetFolderShouldReturnSuccess_WhenFilePathIsValid()
    {
        // arrange
        var fileAdapter = new FakeFileSystemAdapter(
            @"{
                ""name"": ""John"",
                ""age"": 30,
                ""city"": ""New York""
            }");
        var context = TestContextFactory.CreateWithFakeFileAdapter(fileAdapter);

        // act
        var result = context.Run(["file", "./test/file.json", "-t", "./test/ouput"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains("Json file was successfully minified.", result.Output);
    }

    [TestMethod]
    public void Execute_ShouldReturnError_WhenFilePathIsInvalid()
    {
        // arrange
        var fileAdapter = new FakeFileSystemAdapter(string.Empty);
        var context = TestContextFactory.CreateWithFakeFileAdapter(fileAdapter);

        // act
        var result = context.Run(["file", "./invalid/path.json"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(-1, result.ExitCode);
        Assert.Contains("Error: The file './invalid/path.json' does not exist.", result.Output);
    }
}
