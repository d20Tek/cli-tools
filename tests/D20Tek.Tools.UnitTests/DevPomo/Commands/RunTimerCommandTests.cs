using D20Tek.Tools.DevPomo.Commands.RunTimer;

namespace D20Tek.Tools.UnitTests.DevPomo.Commands;

[TestClass]
public class RunTimerCommandTests
{
    [TestMethod]
    public void Execute_WithImmediateQuit_EndsPomoTimer()
    {
        // arrange
        var context = TestContextFactory.Create();
        context.Console.TestInput.PushKey(ConsoleKey.Q);

        // act
        var result = context.Run(["run-timer"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);

        Assert.Contains("Pomodoro Stopped Early", result.Output);
    }

    [TestMethod]
    public void Execute_WithPauseResume_EndsPomoTimer()
    {
        // arrange
        var context = TestContextFactory.Create();
        context.Console.TestInput.PushKey(ConsoleKey.P);
        context.Console.TestInput.PushKey(ConsoleKey.R);
        context.Console.TestInput.PushKey(ConsoleKey.P);
        context.Console.TestInput.PushKey(ConsoleKey.R);
        context.Console.TestInput.PushKey(ConsoleKey.P);
        context.Console.TestInput.PushKey(ConsoleKey.R);
        context.Console.TestInput.PushKey(ConsoleKey.Q);

        // act
        var result = context.Run(["run-timer"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);

        Assert.Contains("Pomodoro Stopped Early", result.Output);
    }

    [TestMethod]
    public void ShowExitMessage_WithCompletedState_ShowsCompleteMessage()
    {
        // arrange
        var console = new TestConsole();
        var state = new TimerState().IncrementPomodoro().IncrementPomodoro();

        // act
        RunTimerCommand.ShowExitMessage(console, state);

        // assert
        Assert.Contains("Pomodoro run ended! You completed 2 pomodoro(s)", console.Output);
    }

    [TestMethod]
    public void ShowExitMessage_WithExitState_ShowsExitMessageAndCompletePomodoros()
    {
        // arrange
        var console = new TestConsole();
        var state = new TimerState().IncrementPomodoro().RequestExit();

        // act
        RunTimerCommand.ShowExitMessage(console, state);

        // assert
        Assert.Contains("Pomodoro Stopped Early", console.Output);
        Assert.Contains("But you completed 1 pomodoro(s) before stopping", console.Output);
    }
}
