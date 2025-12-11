using D20Tek.Tools.DevPomo.Common;
using Spectre.Console;

namespace D20Tek.Tools.UnitTests.DevPomo;

[TestClass]
public class AnsiConsoleExtensionsTests
{
    [TestMethod]
    public void DisplayAppHeader_WithColor_ShowsHeader()
    {
        // arrange
        var console = new TestConsole();

        // act
        console.DisplayAppHeader("Test", Justify.Left, Color.Yellow);

        // assert
        Assert.IsNotNull(console.Output);
    }

    [TestMethod]
    public void ContinueOnAnyKey_WithMessage_Continues()
    {
        // arrange
        var console = new TestConsole();
        console.TestInput.PushTextWithEnter("x");

        // act
        console.ContinueOnAnyKey(["Test message", "Continue?"]);

        // assert
        Assert.Contains("Continue?", console.Output);
    }
}
