using D20Tek.Tools.DevPomo.Commands.RunTimer;

namespace D20Tek.Tools.UnitTests.DevPomo.Commands;

[TestClass]
public class TimerStateTests
{
    [TestMethod]
    public void WithBeep_ShowsMessage()
    {
        // arrange
        var console = new TestConsole();
        var state = new TimerState();

        // act
        var result = state.WithBeep(console, "test message");

        // assert
        Assert.IsNotNull(result);
        Assert.Contains("test message", console.Output);
    }

    [TestMethod]
    public void GetBreakMinutes()
    {
        // arrange
        var state = new TimerState();

        // act
        var result = state.BreakMinutes;

        // assert
        Assert.AreEqual(5, result);
    }

    [TestMethod]
    public void ArePomodorosComplete_UnderCompletionCount_ReturnsFalse()
    {
        // arrange
        var state = new TimerState().IncrementPomodoro();

        // act
        var result = state.ArePomodorosComplete();

        // assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void ArePomodorosComplete_WithCompletionCount_ReturnsTrue()
    {
        // arrange
        var state = new TimerState().IncrementPomodoro().IncrementPomodoro().IncrementPomodoro().IncrementPomodoro();

        // act
        var result = state.ArePomodorosComplete();

        // assert
        Assert.IsTrue(result);
    }
}
