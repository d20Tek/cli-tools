using D20Tek.Spectre.Console.Extensions.Testing;
using D20Tek.Tools.DevKillPort;
using D20Tek.Tools.DevKillPort.Commands;
using D20Tek.Tools.UnitTests.DevKillPort.Fakes;
using Spectre.Console.Cli;

namespace D20Tek.Tools.UnitTests.DevKillPort.Commands;

[TestClass]
public class InteractiveCommandTests
{
    [TestMethod]
    public async Task Execute_WithExitSubcommand_ExitsApp()
    {
        // arrange
        var app = new FakeCommandApp();
        var console = new TestConsole();
        var input = new TestConsoleInput();
        input.PushTextWithEnter("exit");
        console.TestInput = input;

        var command = new InteractiveCommand(app, console);
        var context = new CommandContext([], NullRemainingArguments.Instance, "test", null);

        // act
        var result = await command.ExecuteAsync(context, CancellationToken.None);

        // assert
        Assert.AreEqual(0, result);
        Assert.Contains("Running interactive mode.", console.Output);
        Assert.Contains(Constants.AppPrompt, console.Output);
        Assert.Contains("Thanks for using dev-killport.", console.Output);
    }

    [TestMethod]
    public async Task Execute_WithSubcommandThenExit_RunsCommand()
    {
        // arrange
        var app = new FakeCommandApp();
        var console = new TestConsole();
        var input = new TestConsoleInput();
        input.PushTextWithEnter("list 5000");
        input.PushTextWithEnter("exit");
        console.TestInput = input;

        var command = new InteractiveCommand(app, console);
        var context = new CommandContext([], NullRemainingArguments.Instance, "test", null);

        // act
        var result = await command.ExecuteAsync(context, CancellationToken.None);

        // assert
        Assert.AreEqual(0, result);
        Assert.Contains("Running interactive mode.", console.Output);
        Assert.Contains("Thanks for using dev-killport.", console.Output);
    }

    [TestMethod]
    public async Task Execute_WithFailingSubcommand_ContinuesLoop()
    {
        // arrange
        var app = new FakeCommandApp(expectedResult: -1);
        var console = new TestConsole();
        var input = new TestConsoleInput();
        input.PushTextWithEnter("unknown-command");
        input.PushTextWithEnter("exit");
        console.TestInput = input;

        var command = new InteractiveCommand(app, console);
        var context = new CommandContext([], NullRemainingArguments.Instance, "test", null);

        // act
        var result = await command.ExecuteAsync(context, CancellationToken.None);

        // assert
        Assert.AreEqual(0, result);
        Assert.Contains(Constants.AppPrompt, console.Output);
        Assert.Contains("Thanks for using dev-killport.", console.Output);
    }
}
