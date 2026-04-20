using System.Diagnostics.CodeAnalysis;

namespace D20Tek.Tools.DevKillPort.Services;

[ExcludeFromCodeCoverage]
internal class OperatingSystemAdapter : IOperatingSystemAdapter
{
    public bool IsLinux() => OperatingSystem.IsLinux();

    public bool IsMacOS() => OperatingSystem.IsMacOS();

    public bool IsWindows() => OperatingSystem.IsWindows();
}
