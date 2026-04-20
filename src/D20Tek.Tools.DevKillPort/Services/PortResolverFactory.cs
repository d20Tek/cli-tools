namespace D20Tek.Tools.DevKillPort.Services;

internal static class PortResolverFactory
{
    public static IPortResolver Create(IOperatingSystemAdapter osAdapter, ICommandRunner commandRunner)
    {
        if (osAdapter.IsWindows())
            return new WindowsPortResolver(commandRunner);

        if (osAdapter.IsLinux())
            return new LinuxPortResolver(commandRunner, new ProcFileSystem());

        if (osAdapter.IsMacOS())
            return new MacPortResolver(commandRunner);

        throw new PlatformNotSupportedException(
            "dev-killport is not supported on this platform.");
    }
}
