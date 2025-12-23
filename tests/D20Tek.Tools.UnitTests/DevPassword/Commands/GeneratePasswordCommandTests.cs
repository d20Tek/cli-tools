using D20Tek.Tools.DevPassword.Commands;
using D20Tek.Tools.DevPassword.Contracts;
using D20Tek.Tools.DevPassword.Services;
using System.ComponentModel.DataAnnotations;

namespace D20Tek.Tools.UnitTests.DevPassword.Commands;

[TestClass]
public class GeneratePasswordCommandTests
{
    [TestMethod]
    public void Execute_WithDefaultSettings_GeneratesPassword()
    {
        // arrange
        var context = TestContextFactory.Create();

        // act
        var result = context.Run(["generate"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.ExitCode);
        Assert.Contains("Command completed successfully!", result.Output);
    }

    [TestMethod]
    public void Execute_WithLowLengthSetting_ReturnsErrorMessage()
    {
        // arrange
        var context = TestContextFactory.Create();

        // act
        var result = context.Run(["generate", "-l", "2"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(-1, result.ExitCode);
        Assert.Contains("Error: Password length must be between 4-64 characters.", result.Output);
    }

    [TestMethod]
    public void Execute_WithGreaterLengthSetting_ReturnsErrorMessage()
    {
        // arrange
        var context = TestContextFactory.Create();

        // act
        var result = context.Run(["generate", "-l", "68"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(-1, result.ExitCode);
        Assert.Contains("Error: Password length must be between 4-64 characters.", result.Output);
    }

    [TestMethod]
    public void Execute_WithZeroCountSetting_ReturnsErrorMessage()
    {
        // arrange
        var context = TestContextFactory.Create();

        // act
        var result = context.Run(["generate", "-c", "0"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(-1, result.ExitCode);
        Assert.Contains("Error: Password count must be between 1-100 characters.", result.Output);
    }

    [TestMethod]
    public void Execute_WithGreaterCountSetting_ReturnsErrorMessage()
    {
        // arrange
        var context = TestContextFactory.Create();

        // act
        var result = context.Run(["generate", "-c", "1000"]);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(-1, result.ExitCode);
        Assert.Contains("Error: Password count must be between 1-100 characters.", result.Output);
    }

    [TestMethod]
    public void Execute_WithNoRequiredChars_ReturnsErrorMessage()
    {
        // arrange
        var console = new TestConsole();
        var config = new ConfigurationService(new LowDb<PasswordConfig>(new MemoryStorageAdapter<PasswordConfig>()));
        var writer = new ConsoleVerbosityWriter(console);
        var command = new GeneratePasswordCommand(new PasswordGenerator(), config, writer);
        var context = new CommandContext([], new NullRemainingArguments(), "GeneratePasswordCommand", null);

        config.Set(new PasswordConfig(false, false, false, false));

        // act
        var result = command.Execute(context, new PasswordSettings(), CancellationToken.None);

        // assert
        Assert.AreEqual(-1, result);
        Assert.Contains("must include at least one character set", console.Output);
    }
}
