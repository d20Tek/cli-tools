using D20Tek.Tools.DevKillPort.Services;
using System.Diagnostics.CodeAnalysis;

namespace D20Tek.Tools.UnitTests.DevKillPort.Fakes;

[ExcludeFromCodeCoverage]
internal sealed class FakeCommandRunner : ICommandRunner
{
    private readonly Dictionary<string, string> _responses = new();

    public FakeCommandRunner WithResponse(string command, string arguments, string output)
    {
        _responses[$"{command}|{arguments}"] = output;
        return this;
    }

    public FakeCommandRunner WithResponse(string key, string output)
    {
        _responses[key] = output;
        return this;
    }

    public Task<string> RunAsync(string command, string arguments, CancellationToken ct = default)
    {
        var key = $"{command}|{arguments}";
        if (_responses.TryGetValue(key, out var response))
            return Task.FromResult(response);

        foreach (var kvp in _responses)
        {
            if (key.Contains(kvp.Key))
                return Task.FromResult(kvp.Value);
        }

        return Task.FromResult(string.Empty);
    }
}
