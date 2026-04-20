using D20Tek.Tools.DevKillPort.Services;
using D20Tek.Tools.UnitTests.DevKillPort.Fakes;

namespace D20Tek.Tools.UnitTests.DevKillPort.Services;

[TestClass]
public class PortResolverFactoryTests
{
    private readonly FakeCommandRunner _commandRunner = new();

    [TestMethod]
    public void Create_OnWindows_ReturnsWindowsPortResolver()
    {
        // arrange
        var osAdapter = new FakeOperatingSystemAdapter(isWindows: true);

        // act
        var resolver = PortResolverFactory.Create(osAdapter, _commandRunner);

        // assert
        Assert.IsInstanceOfType<WindowsPortResolver>(resolver);
    }

    [TestMethod]
    public void Create_OnLinux_ReturnsLinuxPortResolver()
    {
        // arrange
        var osAdapter = new FakeOperatingSystemAdapter(isLinux: true);

        // act
        var resolver = PortResolverFactory.Create(osAdapter, _commandRunner);

        // assert
        Assert.IsInstanceOfType<LinuxPortResolver>(resolver);
    }

    [TestMethod]
    public void Create_OnMacOS_ReturnsMacPortResolver()
    {
        // arrange
        var osAdapter = new FakeOperatingSystemAdapter(isMacOS: true);

        // act
        var resolver = PortResolverFactory.Create(osAdapter, _commandRunner);

        // assert
        Assert.IsInstanceOfType<MacPortResolver>(resolver);
    }

    [TestMethod]
    public void Create_OnUnsupportedPlatform_ThrowsPlatformNotSupportedException()
    {
        // arrange
        var osAdapter = new FakeOperatingSystemAdapter();

        // act / assert
        Assert.ThrowsExactly<PlatformNotSupportedException>(
            [ExcludeFromCodeCoverage]() => PortResolverFactory.Create(osAdapter, _commandRunner));
    }
}
