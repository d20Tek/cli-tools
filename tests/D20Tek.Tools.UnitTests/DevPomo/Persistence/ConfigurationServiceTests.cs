using D20Tek.LowDb;
using D20Tek.LowDb.Adapters;
using D20Tek.Tools.DevPomo.Contracts;
using D20Tek.Tools.DevPomo.Persistence;

namespace D20Tek.Tools.UnitTests.DevPomo.Persistence;

[TestClass]
public class ConfigurationServiceTests
{
    [TestMethod]
    public void Get_FromMemory_ReturnsDefaultTimerConfiguration()
    {
        // arrange
        var service = CreateTestService();

        // act
        var result = service.Get();

        // assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsSuccess);
        var value = result.GetValue();
        Assert.AreEqual(25, value.PomodoroMinutes);
        Assert.AreEqual(5, value.BreakMinutes);
        Assert.IsTrue(value.ShowAppTitleBar);
        Assert.IsTrue(value.EnableSound);
        Assert.IsFalse(value.AutostartCycles);
        Assert.IsFalse(value.MinimalOutput);
    }

    [TestMethod]
    public void Set_ValidTimerConfiguration_SavesToInMemoryDb()
    {
        // arrange
        var service = CreateTestService();
        var updated = TimerConfiguration.Create(5, 1, false, false, true, true);

        // act
        var result = service.Set(updated);

        // assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsSuccess);
        var value = result.GetValue();
        Assert.AreEqual(5, value.PomodoroMinutes);
        Assert.AreEqual(1, value.BreakMinutes);
        Assert.IsFalse(value.ShowAppTitleBar);
        Assert.IsFalse(value.EnableSound);
        Assert.IsTrue(value.AutostartCycles);
        Assert.IsTrue(value.MinimalOutput);
    }

    private static IConfigurationService CreateTestService() =>
        new ConfigurationService(
            new LowDb<TimerConfiguration>(
                new MemoryStorageAdapter<TimerConfiguration>()));
}
