using D20Tek.Tools.DevPassword.Contracts;

namespace D20Tek.Tools.UnitTests.DevPassword.Services;

[TestClass]
public class ContractRecordTests
{
    [TestMethod]
    public void PasswordState_WithUpdate_ReturnsNewValue()
    {
        // arrange
        var state = new PasswordState(0, null!, null!);

        // act
        var result = state with
        {
            Length = 10,
            Config = new(),
            Rnd = [ExcludeFromCodeCoverage] (x) => Random.Shared.Next(x),
            CharSet = "testchars",
            Entropy = 42.9
        };

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(10, result.Length);
        Assert.IsNotNull(result.Config);
        Assert.IsNotNull(result.Rnd);
        Assert.AreEqual("testchars", result.CharSet);
        Assert.AreEqual(42.9, result.Entropy);
    }

    [TestMethod]
    public void PasswordResponse_WithUpdate_ReturnsNewValue()
    {
        // arrange
        var response = new PasswordResponse(string.Empty, 0, string.Empty);

        // act
        var result = response with { Password = "P@$$w0rd1", Entropy = 20, Strength = "Weak" };

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual("P@$$w0rd1", result.Password);
        Assert.AreEqual(20, result.Entropy);
        Assert.AreEqual("Weak", result.Strength);
    }
}
