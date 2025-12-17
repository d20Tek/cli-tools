using D20Tek.LowDb;
using D20Tek.LowDb.Adapters;
using D20Tek.Tools.DevPassword.Contracts;
using D20Tek.Tools.DevPassword.Persistence;

namespace D20Tek.Tools.UnitTests.DevPassword.Persistence;

[TestClass]
public class ConfigurationServiceTests
{
    [TestMethod]
    public void Get_FromMemory_ReturnsDefaultPasswordConfig()
    {
        // arrange
        var service = CreateTestService();

        // act
        var result = service.Get();

        // assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsSuccess);
        var value = result.GetValue();
        Assert.IsTrue(value.IncludeLowerCase);
        Assert.IsTrue(value.IncludeUpperCase);
        Assert.IsTrue(value.IncludeNumbers);
        Assert.IsTrue(value.IncludeSymbols);
        Assert.IsFalse(value.ExcludeAmbiguous);
        Assert.IsFalse(value.ExcludeAmbiguous);
        Assert.AreEqual(4, value.RequiredCharsAmount);
    }

    [TestMethod]
    public void Set_ValidTimerConfiguration_SavesToInMemoryDb()
    {
        // arrange
        var service = CreateTestService();
        var updated = new PasswordConfig(true, false, false, false, true, true);

        // act
        var result = service.Set(updated);

        // assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsSuccess);
        var value = result.GetValue();
        Assert.IsTrue(value.IncludeLowerCase);
        Assert.IsFalse(value.IncludeUpperCase);
        Assert.IsFalse(value.IncludeNumbers);
        Assert.IsFalse(value.IncludeSymbols);
        Assert.IsTrue(value.ExcludeAmbiguous);
        Assert.IsTrue(value.ExcludeAmbiguous);
        Assert.AreEqual(1, value.RequiredCharsAmount);
    }

    private static IConfigurationService CreateTestService() =>
        new ConfigurationService(
            new LowDb<PasswordConfig>(
                new MemoryStorageAdapter<PasswordConfig>()));
}
