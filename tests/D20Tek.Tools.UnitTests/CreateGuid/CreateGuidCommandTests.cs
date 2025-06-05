using D20Tek.Spectre.Console.Extensions.Services;
using D20Tek.Spectre.Console.Extensions.Testing;
using D20Tek.Tools.CreateGuid;
using D20Tek.Tools.CreateGuid.Commands;
using D20Tek.Tools.CreateGuid.Services;
using D20Tek.Tools.UnitTests.CreateGuid.Fakes;
using Spectre.Console.Cli;
using TextCopy;

namespace D20Tek.Tools.UnitTests.CreateGuid;

[TestClass]
public class CreateGuidCommandTests
{
    private readonly IGuidFormatter _formatter = new GuidFormatter();
    private readonly IClipboard _clipboard = new FakeClipboard();
    private readonly CommandContext _defaultContext = new([], NullRemainingArguments.Instance, "test", null);

    [TestMethod]
    public void Execute_WithDefaultSettings_CreatesGuid()
    {
        // arrange
        var guid = Guid.NewGuid();
        var context = TestContextFactory.CreateWithGuid(guid);

        // act
        var result = context.Run(["generate"]);

        // assert
        result.AssertValidWithGuid(guid.ToString());
    }

    [TestMethod]
    public void Execute_WithUseUpperCaseSettings_CreatesGuid()
    {
        // arrange
        var guid = Guid.NewGuid();
        var context = TestContextFactory.CreateWithGuid(guid);

        // act
        var result = context.Run(["generate", "--upper"]);

        // assert
        result.AssertValidWithGuid(guid.ToString().ToUpper());
    }

    [TestMethod]
    public void Execute_WithMultipleCountSettings_CreatesGuid()
    {
        // arrange
        Guid[] guids = [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()];

        var context = TestContextFactory.CreateWithGuids(guids);

        // act
        var result = context.Run(["generate", "--count", "3"]);

        // assert
        result.AssertValidWithThreeGuids(guids);
    }

    [TestMethod]
    public void Execute_WithUuidV7Settings_CreatesGuid()
    {
        // arrange
        var guid = Guid.CreateVersion7();
        var context = TestContextFactory.CreateWithGuid(guid);

        // act
        var result = context.Run(["generate", "--uuidv7"]);

        // assert
        result.AssertValidWithGuid(guid.ToString());
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
        var settings = new GuidSettings { CopyToClipboard = true, OutputFile = "./test.txt" };

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

    [TestMethod]
    public void Execute_WithFormatSettings_CreatesGuid()
    {
        // arrange
        var console = new TestConsole();
        var writer = new ConsoleVerbosityWriter(console);

        var guid = Guid.NewGuid();
        var guidGen = new FakeGuidGenerator(guid);
        var command = new CreateGuidCommand(guidGen, _formatter, writer, _clipboard);
        var settings = new GuidSettings { Format = GuidFormat.Number };

        // act
        var result = command.Execute(_defaultContext, settings);

        // assert
        Assert.AreEqual(0, result);
        var output = console.Output;
        StringAssert.Contains(output, guid.ToString("N"));
        Assert.IsFalse(output.Contains(guid.ToString()));
        StringAssert.StartsWith(output, "create-guid: running");
        StringAssert.Contains(output, "Command completed successfully!");
    }
}
