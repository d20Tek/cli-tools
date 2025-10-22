using D20Tek.NuGet.Portfolio;
using D20Tek.NuGet.Portfolio.Common;
using D20Tek.Spectre.Console.Extensions.Testing;

namespace D20Tek.Tools.UnitTests.NuGetPortfolio.Common;

[TestClass]
public class AskIfDefaultTests
{

    [TestMethod]
    public void AskIfDefault_WithValue_ShouldReturnValue()
    {
        // arrange
        var console = new TestConsole();

        // act
        var result = console.AskIfDefault<string>("test", "My label:");

        // assert
        Assert.AreEqual("test", result);
        Assert.DoesNotContain("My label:", console.Output);
        Assert.DoesNotContain(Globals.AppPrompt, console.Output);
    }

    [TestMethod]
    public void AskIfDefault_WithNullValue_ShouldReturnInputValue()
    {
        // arrange
        var console = new TestConsole();
        console.TestInput.PushTextWithEnter("user input");

        // act
        var result = console.AskIfDefault<string>(null!, "My label:");

        // assert
        Assert.AreEqual("user input", result);
        Assert.Contains("My label:", console.Output);
        Assert.Contains(Globals.AppPrompt, console.Output);
    }

    [TestMethod]
    public void AskIfDefault_WithEmptyString_ShouldReturnInputValue()
    {
        // arrange
        var console = new TestConsole();
        console.TestInput.PushTextWithEnter("user input");

        // act
        var result = console.AskIfDefault<string>(string.Empty, "My label:");

        // assert
        Assert.AreEqual("user input", result);
        Assert.Contains("My label:", console.Output);
        Assert.Contains(Globals.AppPrompt, console.Output);
    }

    [TestMethod]
    public void AskIfDefault_WithIntValue_ShouldReturnValue()
    {
        // arrange
        var console = new TestConsole();

        // act
        var result = console.AskIfDefault<int>(42, "My label:");

        // assert
        Assert.AreEqual(42, result);
    }

    [TestMethod]
    public void AskIfDefault_WithIntDefaultValue_ShouldReturnInputValue()
    {
        // arrange
        var console = new TestConsole();
        console.TestInput.PushTextWithEnter("101");

        // act
        var result = console.AskIfDefault<int>(0, "My label:");

        // assert
        Assert.AreEqual(101, result);
    }
}
