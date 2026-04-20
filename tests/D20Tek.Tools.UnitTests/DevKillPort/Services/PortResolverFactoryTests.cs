using D20Tek.Tools.DevKillPort.Services;
using D20Tek.Tools.UnitTests.DevKillPort.Fakes;

namespace D20Tek.Tools.UnitTests.DevKillPort.Services;

[TestClass]
public class PortResolverFactoryTests
{
    [TestMethod]
    public void Create_ReturnsNonNullResolver()
    {
        // arrange
        var commandRunner = new FakeCommandRunner();

        // act
        var resolver = PortResolverFactory.Create(commandRunner);

        // assert
        Assert.IsNotNull(resolver);
    }

    [TestMethod]
    [ExcludeFromCodeCoverage]
    public void Create_OnCurrentPlatform_ReturnsExpectedType()
    {
        // arrange
        var commandRunner = new FakeCommandRunner();

        // act
        var resolver = PortResolverFactory.Create(commandRunner);

        // assert
        if (OperatingSystem.IsWindows())
            Assert.IsInstanceOfType<WindowsPortResolver>(resolver);
        else if (OperatingSystem.IsLinux())
            Assert.IsInstanceOfType<LinuxPortResolver>(resolver);
        else if (OperatingSystem.IsMacOS())
            Assert.IsInstanceOfType<MacPortResolver>(resolver);
    }
}
