using D20Tek.Functional;
using D20Tek.NuGet.Portfolio;
using D20Tek.NuGet.Portfolio.Common.Controls;
using D20Tek.Spectre.Console.Extensions.Testing;
using System.Diagnostics.CodeAnalysis;

namespace D20Tek.Tools.UnitTests.NuGetPortfolio.Common;

[TestClass]
public class ResultPresenterTests
{

    [TestMethod]
    public void Render_WithSuccess_ShowsSuccessMessage()
    {
        // arrange
        var console = new TestConsole();
        var result = Result<int>.Success(115);

        // act
        var code = result.Render(console, x => $"Result={x}");

        // assert
        Assert.AreEqual(Globals.S_OK, code);
        Assert.StartsWith("Success:", console.Output);
        Assert.EndsWith("Result=115\n", console.Output);
    }

    [TestMethod]
    public void Render_WithSingleError_ShowsErrorMessage()
    {
        // arrange
        var console = new TestConsole();
        var result = Result<int>.Failure(Error.Validation("Test", "Invalid value."));

        // act
        var code = result.Render(console, [ExcludeFromCodeCoverage] (x) => $"Result={x}");

        // assert
        Assert.AreEqual(Globals.E_FAIL, code);
        Assert.StartsWith("Error:", console.Output);
        Assert.EndsWith("Invalid value.\n", console.Output);
    }

    [TestMethod]
    public void Render_WithMultipcleErrors_ShowsErrorMessages()
    {
        // arrange
        var console = new TestConsole();
        var result = Result<int>.Failure(
        [
            Error.Validation("Test", "Invalid value."),
            Error.Validation("Two", "Second error."),
            Error.Validation("Last", "Another error.")
        ]);

        // act
        var code = result.Render(console, [ExcludeFromCodeCoverage] (x) => $"Result={x}");

        // assert
        Assert.AreEqual(Globals.E_FAIL, code);
        Assert.StartsWith("Multiple error messages:", console.Output);
        Assert.Contains("- Invalid value.", console.Output);
        Assert.Contains("- Second error.", console.Output);
        Assert.Contains("- Another error.", console.Output);
    }

    [TestMethod]
    public async Task RenderAsync_WithSuccess_ShowsSuccessMessage()
    {
        // arrange
        var console = new TestConsole();
        var result = Task.FromResult(Result<int>.Success(115));

        // act
        var code = await result.RenderAsync(console, x => $"Result={x}");

        // assert
        Assert.AreEqual(Globals.S_OK, code);
        Assert.StartsWith("Success:", console.Output);
        Assert.EndsWith("Result=115\n", console.Output);
    }

    [TestMethod]
    public async Task RenderAsync_WithSingleError_ShowsErrorMessage()
    {
        // arrange
        var console = new TestConsole();
        var result = Task.FromResult(Result<int>.Failure(Error.Validation("Test", "Invalid value.")));

        // act
        var code = await result.RenderAsync(console, [ExcludeFromCodeCoverage] (x) => $"Result={x}");

        // assert
        Assert.AreEqual(Globals.E_FAIL, code);
        Assert.StartsWith("Error:", console.Output);
        Assert.EndsWith("Invalid value.\n", console.Output);
    }

    [TestMethod]
    public async Task RenderAsync_WithMultipcleErrors_ShowsErrorMessages()
    {
        // arrange
        var console = new TestConsole();
        var result = Task.FromResult(Result<int>.Failure(
        [
            Error.Validation("Test", "Invalid value."),
            Error.Validation("Two", "Second error."),
            Error.Validation("Last", "Another error.")
        ]));

        // act
        var code = await result.RenderAsync(console, [ExcludeFromCodeCoverage] (x) => $"Result={x}");

        // assert
        Assert.AreEqual(Globals.E_FAIL, code);
        Assert.StartsWith("Multiple error messages:", console.Output);
        Assert.Contains("- Invalid value.", console.Output);
        Assert.Contains("- Second error.", console.Output);
        Assert.Contains("- Another error.", console.Output);
    }
}