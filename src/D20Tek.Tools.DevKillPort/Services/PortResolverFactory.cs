namespace D20Tek.Tools.DevKillPort.Services;

internal static class PortResolverFactory
{
    public static IPortResolver Create(ICommandRunner commandRunner)
    {
        if (OperatingSystem.IsWindows())
            return new WindowsPortResolver(commandRunner);

        if (OperatingSystem.IsLinux())
            return new LinuxPortResolver(commandRunner, new ProcFileSystem());

        if (OperatingSystem.IsMacOS())
            return new MacPortResolver(commandRunner);

        throw new PlatformNotSupportedException(
            "dev-killport is not supported on this platform.");
    }
}
