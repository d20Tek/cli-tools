using D20Tek.Spectre.Console.Extensions.Services;
using D20Tek.Spectre.Console.Extensions.Testing;
using D20Tek.Tools.CreateGuid.Commands;
using D20Tek.Tools.CreateGuid.Services;
using D20Tek.Tools.UnitTests.Fakes;
using Spectre.Console.Cli;
using TextCopy;

namespace D20Tek.Tooks.UnitTests.CreateGuid;

[TestClass]
public class CreateGuidCommandTests
{
    private readonly IGuidFormatter _formatter = new GuidFormatter();
    private readonly IClipboard _clipboard = new Clipboard();
    private readonly CommandContext _defaultContext = new([], NullRemainingArguments.Instance, "test", null);

    [TestMethod]
    public void Execute_WithDefaultSettings_CreatesGuid()
    {
        // arrange
        var console = new TestConsole();
        var writer = new ConsoleVerbosityWriter(console);

        var guid = Guid.NewGuid();
        var guidGen = new FakeGuidGenerator(guid);
        var command = new CreateGuidCommand(guidGen, _formatter, writer, _clipboard);
        var settings = new GuidSettings();

        // act
        var result = command.Execute(_defaultContext, settings);

        // assert
        Assert.AreEqual(0, result);
        var output = console.Output;
        StringAssert.Contains(output, guid.ToString());
        StringAssert.StartsWith(output, "create-guid: running");
        StringAssert.Contains(output, "Command completed successfully!");
    }

    [TestMethod]
    public void Execute_WithUseUpperCaseSettings_CreatesGuid()
    {
        // arrange
        var console = new TestConsole();
        var writer = new ConsoleVerbosityWriter(console);

        var guid = Guid.NewGuid();
        var guidGen = new FakeGuidGenerator(guid);
        var command = new CreateGuidCommand(guidGen, _formatter, writer, _clipboard);
        var settings = new GuidSettings { UsesUpperCase = true };

        // act
        var result = command.Execute(_defaultContext, settings);

        // assert
        Assert.AreEqual(0, result);
        var output = console.Output;
        StringAssert.Contains(output, guid.ToString().ToUpper());
        StringAssert.StartsWith(output, "create-guid: running");
        StringAssert.Contains(output, "Command completed successfully!");
    }

    [TestMethod]
    public void Execute_WithMultipleCountSettings_CreatesGuid()
    {
        // arrange
        var console = new TestConsole();
        var writer = new ConsoleVerbosityWriter(console);

        var guid = Guid.NewGuid();
        var guidGen = new FakeGuidGenerator([guid, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()]);
        var command = new CreateGuidCommand(guidGen, _formatter, writer, _clipboard);
        var settings = new GuidSettings { Count = 3 };

        // act
        var result = command.Execute(_defaultContext, settings);

        // assert
        Assert.AreEqual(0, result);
        var output = console.Output;
        StringAssert.Contains(output, guid.ToString());
        StringAssert.StartsWith(output, "create-guid: running");
        StringAssert.Contains(output, "Command completed successfully!");
    }

    [TestMethod]
    public void Execute_WithEmptyGuidSettings_CreatesGuid()
    {
        // arrange
        var console = new TestConsole();
        var writer = new ConsoleVerbosityWriter(console);

        var guidGen = new GuidGenerator();
        var command = new CreateGuidCommand(guidGen, _formatter, writer, _clipboard);
        var settings = new GuidSettings { UsesEmptyGuid = true };

        // act
        var result = command.Execute(_defaultContext, settings);

        // assert
        Assert.AreEqual(0, result);
        var output = console.Output;
        StringAssert.Contains(output, Guid.Empty.ToString());
        StringAssert.StartsWith(output, "create-guid: running");
        StringAssert.Contains(output, "Command completed successfully!");
    }

    [TestMethod]
    public void Execute_WithCopyToClipboardSettings_CreatesGuidOnClipboard()
    {
        // arrange
        var console = new TestConsole();
        var writer = new ConsoleVerbosityWriter(console);

        var guid = Guid.NewGuid();
        var guidGen = new FakeGuidGenerator(guid);
        var command = new CreateGuidCommand(guidGen, _formatter, writer, _clipboard);
        var settings = new GuidSettings { CopyToClipboard = true, OutputFile = ".\\test.txt" };

        // act
        var result = command.Execute(_defaultContext, settings);

        // assert
        Assert.AreEqual(0, result);
        var output = console.Output;
        StringAssert.Contains(output, guid.ToString());
        StringAssert.StartsWith(output, "create-guid: running");
        StringAssert.Contains(output, "Command completed successfully!");
        StringAssert.Contains(_clipboard.GetText(), guid.ToString());
    }
}
