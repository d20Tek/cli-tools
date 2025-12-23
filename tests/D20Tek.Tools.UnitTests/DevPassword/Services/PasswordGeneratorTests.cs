using D20Tek.Tools.DevPassword.Contracts;
using D20Tek.Tools.DevPassword.Services;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace D20Tek.Tools.UnitTests.DevPassword.Services;

[ExcludeFromCodeCoverage]
[TestClass]
public class PasswordGeneratorTests
{
    [TestMethod]
    [DataRow(true, true, true, true, 25, 160, "Very Strong", "")]
    [DataRow(true, false, true, true, 20, 120, "Very Strong", "ABCDEFGHIJKLMNOPQRSTUVWXYZ")]
    [DataRow(false, true, true, true, 20, 120, "Very Strong", "abcdefghijklmnopqrstuvwxyz")]
    [DataRow(true, true, false, true, 25, 150, "Very Strong", "0123456789")]
    [DataRow(true, true, true, false, 50, 290, "Very Strong", "!\\\"#$%&'()*+,-./:;<=>?@[\\\\]^_`{|}~\"")]
    [DataRow(true, true, true, false, 6, 30, "Very Weak", "!\\\"#$%&'()*+,-./:;<=>?@[\\\\]^_`{|}~\"")]
    [DataRow(true, true, true, false, 8, 40, "Weak", "!\\\"#$%&'()*+,-./:;<=>?@[\\\\]^_`{|}~\"")]
    [DataRow(true, true, true, false, 10, 50, "Good", "!\\\"#$%&'()*+,-./:;<=>?@[\\\\]^_`{|}~\"")]
    public void Generate_WithIncludeFlags_ReturnsPasswordResponse(
        bool lower,
        bool upper,
        bool numbers,
        bool symbols,
        int length,
        double entropy,
        string strength,
        string exclusions)
    {
        // arrange
        var config = new PasswordConfig(lower, upper, numbers, symbols);
        var state = new PasswordState(length, 1, config, RndGen);
        var generator = new PasswordGenerator();

        // act
        var response = generator.Generate(state);
        Console.WriteLine(response.First().Password);
        
        // assert
        Assert.IsNotNull(response);
        Assert.AreEqual(1, response.Count());
        var password = response.First();
        Assert.AreEqual(length, password.Password.Length);
        Assert.IsGreaterThan(entropy, password.Entropy);
        Assert.AreEqual(strength, password.Strength);
        Assert.IsFalse(password.Password.Any(c => exclusions.Contains(c)));
    }

    [TestMethod]
    public void Generate_WithExclusions_ReturnsPasswordResponse()
    {
        // arrange
        var exclusions = "iIl1L| o0O `'-_\":;.,<>()[]{}";
        var config = new PasswordConfig(excludeAmbiguous: true, excludeBrackets: true);
        var state = new PasswordState(12, 1, config, RndGen);
        var generator = new PasswordGenerator();

        // act
        var response = generator.Generate(state);
        Console.WriteLine(response.First().Password);

        // assert
        Assert.IsNotNull(response);
        var password = response.First();
        Assert.AreEqual(12, password.Password.Length);
        Assert.IsGreaterThan(70, password.Entropy);
        Assert.AreEqual("Strong", password.Strength);
        Assert.IsFalse(password.Password.Any(c => exclusions.Contains(c)));
    }

    [TestMethod]
    public void Generate_WithMultipleCount_ReturnsMultipleResponses()
    {
        // arrange
        var config = new PasswordConfig();
        var state = new PasswordState(20, 5, config, RndGen);
        var generator = new PasswordGenerator();

        // act
        var response = generator.Generate(state);

        // assert
        Assert.IsNotNull(response);
        Assert.AreEqual(5, response.Count());
        var password = response.First();
        Assert.AreEqual(20, password.Password.Length);
    }

    private static int RndGen(int x) => Random.Shared.Next(x);
}
