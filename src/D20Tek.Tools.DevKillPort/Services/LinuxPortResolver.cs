using D20Tek.Tools.DevKillPort.Contracts;

namespace D20Tek.Tools.DevKillPort.Services;

internal sealed class LinuxPortResolver(ICommandRunner commandRunner, IProcFileSystem procFs) : IPortResolver
{
    private readonly ICommandRunner _commandRunner = commandRunner;
    private readonly ProcPortScanner _procScanner = new(procFs);

    public async Task<IReadOnlyList<PortProcessInfo>> ListAllAsync(
        PortQueryOptions options,
        CancellationToken ct = default)
    {
        var results = await SsPortScanner.TryListAllAsync(options, _commandRunner, ct);
        if (results.Count > 0) return results;

        results = await LsofPortScanner.TryListAllAsync(options, _commandRunner, ct);
        if (results.Count > 0) return results;

        return ListAllWithProc(options);
    }

    internal List<PortProcessInfo> ListAllWithProc(PortQueryOptions options) => _procScanner.ListAllWithProc(options);

    public async Task<IReadOnlyList<PortProcessInfo>> FindAsync(
        int port,
        PortQueryOptions options,
        CancellationToken ct = default)
    {
        var results = await SsPortScanner.TryFindAsync(port, options, _commandRunner, ct);
        if (results.Count > 0) return results;

        results = await LsofPortScanner.TryFindAsync(port, options, _commandRunner, ct);
        if (results.Count > 0) return results;

        return FindWithProc(port, options);
    }

    internal List<PortProcessInfo> FindWithProc(int port, PortQueryOptions options) => _procScanner.FindWithProc(port, options);
}
