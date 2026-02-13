using D20Tek.Tools.JsonMinify;

namespace D20Tek.Tools.UnitTests.JsonMinify;

[TestClass]
public class End2EndTests
{
    [TestMethod]
    public async Task Run_WithFileCommand()
    {
        // arrange
        await Task.Delay(200, CancellationToken.None);
        string[] args = ["file", "./JsonMinify/test-data/weather.json"];

        // act
        var result = await CommandAppE2ERunner.RunAsync(Program.Main, args);

        // assert
        Assert.AreEqual(0, result.ExitCode);
    }

    [TestMethod]
    public async Task Run_WithFolderCommand()
    {
        // arrange
        await Task.Delay(100, CancellationToken.None);
        var targetFolder = "./JsonMinify/test-data/output";
        string[] args = ["folder", "./JsonMinify/test-data", "-t", targetFolder];
        if (Directory.Exists(targetFolder)) Directory.Delete(targetFolder, true);

        // act
        var result = await CommandAppE2ERunner.RunAsync(Program.Main, args);

        // assert
        Assert.AreEqual(0, result.ExitCode);
    }
}
