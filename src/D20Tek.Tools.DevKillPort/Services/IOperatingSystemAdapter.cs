namespace D20Tek.Tools.DevKillPort.Services;

internal interface IOperatingSystemAdapter
{
    bool IsWindows();

    bool IsLinux();

    bool IsMacOS();
}
