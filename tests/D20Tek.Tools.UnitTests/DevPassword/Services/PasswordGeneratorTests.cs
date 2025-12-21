using D20Tek.Tools.DevPassword.Contracts;
using D20Tek.Tools.DevPassword.Services;

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
    public void Generate_WithIncludeFlags_ReturnsString(
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
        var state = new PasswordState(length, config, RndGen);
        var generator = new PasswordGenerator();

        // act
        var response = generator.Generate(state);
        Console.WriteLine(response.Password);

        // assert
        Assert.IsNotNull(response);
        Assert.AreEqual(length, response.Password.Length);
        Assert.IsGreaterThan(entropy, response.Entropy);
        Assert.AreEqual(strength, response.Strength);
        Assert.IsFalse(response.Password.Any(c => exclusions.Contains(c)));
    }

    [TestMethod]
    public void Generate_WithExclusions_ReturnsString()
    {
        // arrange
        var exclusions = "iIl1L| o0O `'-_\":;.,<>()[]{}";
        var config = new PasswordConfig(excludeAmbiguous: true, excludeBrackets: true);
        var state = new PasswordState(12, config, RndGen);
        var generator = new PasswordGenerator();

        // act
        var response = generator.Generate(state);
        Console.WriteLine(response.Password);

        // assert
        Assert.IsNotNull(response);
        Assert.AreEqual(12, response.Password.Length);
        Assert.IsGreaterThan(70, response.Entropy);
        Assert.AreEqual("Strong", response.Strength);
        Assert.IsFalse(response.Password.Any(c => exclusions.Contains(c)));
    }

    private static int RndGen(int x) => Random.Shared.Next(x);
}
