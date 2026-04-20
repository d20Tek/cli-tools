using D20Tek.Tools.DevKillPort.Services;

namespace D20Tek.Tools.UnitTests.DevKillPort.Fakes;

[ExcludeFromCodeCoverage]
internal sealed class ThrowingCommandRunner : ICommandRunner
{
    public Task<string> RunAsync(string command, string arguments, CancellationToken ct = default) =>
        throw new InvalidOperationException($"Command '{command}' failed.");
}
