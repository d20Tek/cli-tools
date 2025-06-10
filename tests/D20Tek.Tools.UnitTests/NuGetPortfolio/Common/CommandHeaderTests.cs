using D20Tek.NuGet.Portfolio.Common.Controls;
using D20Tek.Spectre.Console.Extensions.Testing;

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
        StringAssert.StartsWith(console.Output, "── My…");
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
        StringAssert.StartsWith(console.Output, "── Gre…");
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
        StringAssert.StartsWith(console.Output, "── My Header");
    }
}
