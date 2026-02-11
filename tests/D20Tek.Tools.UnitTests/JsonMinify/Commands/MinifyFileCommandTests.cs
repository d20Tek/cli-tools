using D20Tek.Tools.UnitTests.DevPassword.Fakes;

namespace D20Tek.Tools.UnitTests.JsonMinify.Commands;

[TestClass]
public class MinifyFileCommandTests
{
    [TestMethod]
    public void Execute_ShouldReturnSuccess_WhenFilePathIsValid()
    {
        // arrange
        var fileAdapter = new FakeFileAdapter(
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
}
