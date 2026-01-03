namespace D20Tek.Tools.UnitTests.DevPassword.Commands;

[TestClass]
public class UpdateConfigCommandTests
{
    [TestMethod]
    public void Execute_WithDefault_SavesPasswordConfig()
    {
        // arrange
        var context = TestContextFactory.CreateWithMemoryLowDb();
        context.Console.TestInput.PushTextWithEnter("y");
        context.Console.TestInput.PushTextWithEnter("y");
        context.Console.TestInput.PushTextWithEnter("y");
        context.Console.TestInput.PushTextWithEnter("y");
        context.Console.TestInput.PushTextWithEnter("n");
        context.Console.TestInput.PushTextWithEnter("n");

        // act
        var result = context.Run(["configure"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
    }

    [TestMethod]
    public void Execute_WithValidInput_SavesPasswordConfig()
    {
        // arrange
        var context = TestContextFactory.CreateWithMemoryLowDb();
        context.Console.TestInput.PushTextWithEnter("y");
        context.Console.TestInput.PushTextWithEnter("n");
        context.Console.TestInput.PushTextWithEnter("y");
        context.Console.TestInput.PushTextWithEnter("y");
        context.Console.TestInput.PushTextWithEnter("n");
        context.Console.TestInput.PushTextWithEnter("y");

        // act
        var result = context.Run(["configure"]);

        // assert
        Console.WriteLine(result.ExitCode);
        Console.WriteLine(result.Output);

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
    }
}
