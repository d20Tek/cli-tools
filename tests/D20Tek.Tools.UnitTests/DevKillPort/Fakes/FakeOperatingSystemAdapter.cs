using D20Tek.Tools.DevKillPort.Services;

namespace D20Tek.Tools.UnitTests.DevKillPort.Fakes;

internal sealed class FakeOperatingSystemAdapter(
    bool isWindows = false,
    bool isLinux = false,
    bool isMacOS = false) : IOperatingSystemAdapter
{
    public bool IsWindows() => isWindows;

    public bool IsLinux() => isLinux;

    public bool IsMacOS() => isMacOS;
}
