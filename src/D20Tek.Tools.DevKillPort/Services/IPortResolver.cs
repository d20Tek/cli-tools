using D20Tek.Tools.DevKillPort.Contracts;

namespace D20Tek.Tools.DevKillPort.Services;

internal interface IPortResolver
{
    Task<IReadOnlyList<PortProcessInfo>> FindAsync(int port, PortQueryOptions options, CancellationToken ct = default);
}
