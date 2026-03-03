using D20Tek.Tools.DevLog;

namespace D20Tek.Tools.UnitTests.DevLog;

[TestClass]
[ExcludeFromCodeCoverage]
public sealed class End2EndTests
{
    private string _testFolder = string.Empty;

    [TestInitialize]
    public void SetUp()
    {
        _testFolder = Path.Combine(Path.GetTempPath(), "devlog-e2e-tests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testFolder);
    }

    [TestCleanup]
    public void TearDown()
    {
        if (Directory.Exists(_testFolder)) Directory.Delete(_testFolder, true);
    }

    [TestMethod]
    public async Task Run_ListCommand_WithEmptyFolder_ReturnsSuccess()
    {
        // arrange
        await Task.Delay(100, TestContext.CancellationToken);
        string[] args = ["list", "-f", _testFolder];

        // act
        var result = await CommandAppE2ERunner.RunAsync(Program.Main, args);

        // assert
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains("No dev-log files found", result.Output);
    }

    [TestMethod]
    public async Task Run_ListCommand_WithExistingFiles_DisplaysFiles()
    {
        // arrange
        await Task.Delay(200, TestContext.CancellationToken);
        Directory.CreateDirectory(_testFolder);
        var filePath = Path.Combine(_testFolder, "dev-log-20250105.md");
        File.WriteAllText(filePath, "## Week of January 5, 2025\n\n### TestProject\n- Item 1");
        string[] args = ["list", "-f", _testFolder];

        // act
        var result = await CommandAppE2ERunner.RunAsync(Program.Main, args);

        // assert
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains("dev-log-20250105.md", result.Output);
    }

    [TestMethod]
    public async Task Run_ViewCommand_WithMissingFile_ReturnsError()
    {
        // arrange
        await Task.Delay(300, TestContext.CancellationToken);
        string[] args = ["view", "-f", _testFolder];

        // act
        var result = await CommandAppE2ERunner.RunAsync(Program.Main, args);

        // assert
        Assert.AreEqual(-1, result.ExitCode);
        Assert.Contains("Error:", result.Output);
    }

    [TestMethod]
    public async Task Run_ViewCommand_WithExistingFile_ReturnsSuccess()
    {
        // arrange
        await Task.Delay(400, TestContext.CancellationToken);
        Directory.CreateDirectory(_testFolder);
        var today = DateOnly.FromDateTime(DateTime.Today);
        var weekStart = today.AddDays(-(int)today.DayOfWeek);
        var fileName = string.Format(Constants.FileNameFormat, weekStart);
        var filePath = Path.Combine(_testFolder, fileName);
        File.WriteAllText(filePath, $"## Week of {weekStart:MMMM d, yyyy}\n\n### TestProject\n- Item 1");
        string[] args = ["view", "-f", _testFolder];

        // act
        var result = await CommandAppE2ERunner.RunAsync(Program.Main, args);

        // assert
        Assert.AreEqual(0, result.ExitCode);
    }

    public TestContext TestContext { get; set; } = null!;
}
