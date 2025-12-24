using D20Tek.Tools.Common.Controls;

namespace D20Tek.Tools.UnitTests.NuGetPortfolio.Common;

[TestClass]
public class CommandHeaderTests
{
    [TestMethod]
    public void Render_WithTitleAndDefaults_ShouldPrintHeader()
    {
        // arrange
        var console = new TestConsole();
        var header = new CommandHeader(console);

        // act
        header.Render("My Header");

        // assert
        Assert.StartsWith("── My…", console.Output);
    }

    [TestMethod]
    public void Render_WithDifferentColor_ShouldPrintHeader()
    {
        // arrange
        var console = new TestConsole();
        var header = new CommandHeader(console);

        // act
        header.Render("Green Header", "green");

        // assert
        Assert.StartsWith("── Gre…", console.Output);
    }

    [TestMethod]
    public void Render_WithRightPad_ShouldPrintFullHeader()
    {
        // arrange
        var console = new TestConsole();
        var header = new CommandHeader(console);

        // act
        header.Render("My Header", rightPad: 10);

        // assert
        Assert.StartsWith("── My Header", console.Output);
    }
}
