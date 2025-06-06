using D20Tek.Tools.UnitTests.CreateGuid.Fakes;

namespace D20Tek.Tools.UnitTests.CreateGuid;

[TestClass]
public class CreateGuidCommandTests
{
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
        var context = TestContextFactory.CreateWithGuid(Guid.Empty);

        // act
        var result = context.Run(["generate", "--empty"]);

        // assert
        result.AssertValidWithGuid(Guid.Empty.ToString());
    }

    [TestMethod]
    public void Execute_WithCopyToClipboardSettings_CreatesGuidOnClipboard()
    {
        // arrange
        var clipboard = new FakeClipboard();
        var guid = Guid.NewGuid();
        var context = TestContextFactory.CreateWithClipboard(guid, clipboard);

        // act
        var result = context.Run(["generate", "--clipboard-copy", "--output", "./test.txt"]);

        // assert
        result.AssertValidWithGuid(guid.ToString());
        StringAssert.Contains(clipboard.GetText(), guid.ToString());
    }

    [TestMethod]
    public void Execute_WithFormatSettings_CreatesGuid()
    {
        // arrange
        var guid = Guid.NewGuid();
        var context = TestContextFactory.CreateWithGuid(guid);

        // act
        var result = context.Run(["generate", "--format", "Number"]);

        // assert
        result.AssertValidWithGuid(guid.ToString("N"));
    }
}
