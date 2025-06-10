using D20Tek.NuGet.Portfolio.Features;
using D20Tek.Spectre.Console.Extensions.Testing;
using D20Tek.Tools.UnitTests.NuGetPortfolio.Fakes;
using Spectre.Console.Cli;

namespace D20Tek.Tools.UnitTests.NuGetPortfolio.Features;

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
        var result = await command.ExecuteAsync(context);

        // assert
        Assert.AreEqual(0, result);
        StringAssert.Contains(console.Output, "Running interactive mode.");
        StringAssert.EndsWith(console.Output, "Bye!\n");
        StringAssert.Contains(console.Output, "nu-port>");
    }

    [TestMethod]
    public async Task Execute_WithSubcommandThenExit_RunsCommand()
    {
        // arrange
        var app = new FakeCommandApp();
        var console = new TestConsole();
        var input = new TestConsoleInput();
        input.PushTextWithEnter("coll add --name foo-bar");
        input.PushTextWithEnter("exit");
        console.TestInput = input;

        var command = new InteractiveCommand(app, console);
        var context = new CommandContext([], NullRemainingArguments.Instance, "test", null);

        // act
        var result = await command.ExecuteAsync(context);

        // assert
        Assert.AreEqual(0, result);
        StringAssert.Contains(console.Output, "Running interactive mode.");
        StringAssert.EndsWith(console.Output, "Bye!\n");
        StringAssert.Contains(console.Output, "nu-port>");
    }
}
