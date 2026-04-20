using D20Tek.Tools.DevKillPort.Services;
using System.Diagnostics.CodeAnalysis;

namespace D20Tek.Tools.UnitTests.DevKillPort.Fakes;

[ExcludeFromCodeCoverage]
internal sealed class FakeProcessTerminator : IProcessTerminator
{
    private bool _killResult = true;
    public int KillCallCount { get; private set; }
    public List<int> KilledPids { get; } = [];

    public FakeProcessTerminator WithKillResult(bool result)
    {
        _killResult = result;
        return this;
    }

    public Task<bool> KillAsync(int pid, bool force, CancellationToken ct = default)
    {
        KillCallCount++;
        KilledPids.Add(pid);
        return Task.FromResult(_killResult);
    }
}
