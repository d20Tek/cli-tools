using D20Tek.Tools.DevKillPort.Contracts;
using D20Tek.Tools.DevKillPort.Services;

namespace D20Tek.Tools.UnitTests.DevKillPort.Fakes;

[ExcludeFromCodeCoverage]
internal sealed class FakePortResolver : IPortResolver
{
    private IReadOnlyList<PortProcessInfo> _results = [];
    private readonly Queue<IReadOnlyList<PortProcessInfo>> _resultSequence = new();
    private int _callCount;

    public int CallCount => _callCount;

    public FakePortResolver WithResults(IReadOnlyList<PortProcessInfo> results)
    {
        _results = results;
        return this;
    }

    public FakePortResolver ThenReturns(IReadOnlyList<PortProcessInfo> results)
    {
        _resultSequence.Enqueue(results);
        return this;
    }

    public Task<IReadOnlyList<PortProcessInfo>> FindAsync(
        int port,
        PortQueryOptions options,
        CancellationToken ct = default)
    {
        _callCount++;
        var result = _resultSequence.Count > 0 ? _resultSequence.Dequeue() : _results;
        return Task.FromResult(result);
    }

    public Task<IReadOnlyList<PortProcessInfo>> ListAllAsync(
        PortQueryOptions options,
        CancellationToken ct = default)
    {
        _callCount++;
        var result = _resultSequence.Count > 0 ? _resultSequence.Dequeue() : _results;
        return Task.FromResult(result);
    }
}
