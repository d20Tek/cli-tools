namespace D20Tek.Tools.UnitTests.DevPomo.Commands;

[TestClass]
public class UpdateConfigCommandTests
{
    [TestMethod]
    public void Execute_WithValidInput_SavesTimerConfiguration()
    {
        // arrange
        var context = TestContextFactory.Create();
        context.Console.TestInput.PushTextWithEnter("10");
        context.Console.TestInput.PushTextWithEnter("2");
        context.Console.TestInput.PushTextWithEnter("y");
        context.Console.TestInput.PushTextWithEnter("y");
        context.Console.TestInput.PushTextWithEnter("n");
        context.Console.TestInput.PushTextWithEnter("n");

        // act
        var result = context.Run(["configure"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);

        Assert.Contains("10", result.Output);
        Assert.Contains("2", result.Output);
    }

    [TestMethod]
    public void Execute_WithInvalidTimes_ShowsErrors()
    {
        // arrange
        var context = TestContextFactory.Create();
        context.Console.TestInput.PushTextWithEnter("1");
        context.Console.TestInput.PushTextWithEnter("1000");
        context.Console.TestInput.PushTextWithEnter("10");
        context.Console.TestInput.PushTextWithEnter("0");
        context.Console.TestInput.PushTextWithEnter("100");
        context.Console.TestInput.PushTextWithEnter("2");
        context.Console.TestInput.PushTextWithEnter("y");
        context.Console.TestInput.PushTextWithEnter("y");
        context.Console.TestInput.PushTextWithEnter("n");
        context.Console.TestInput.PushTextWithEnter("n");

        // act
        var result = context.Run(["configure"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);

        Assert.Contains("Pomodoro duration must be between 5 and 120 minutes", result.Output);
        Assert.Contains("Pomodoro break must be between 1 and 20 minutes", result.Output);
        Assert.Contains("10", result.Output);
        Assert.Contains("2", result.Output);
    }
}
