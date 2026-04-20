using D20Tek.Tools.DevKillPort.Services;

namespace D20Tek.Tools.UnitTests.DevKillPort.Fakes;

[ExcludeFromCodeCoverage]
internal sealed class ThrowingProcFileSystem : IProcFileSystem
{
    public bool Exists(string path) => throw new InvalidOperationException("Exists failed.");

    public string[] ReadAllLines(string path) => throw new InvalidOperationException("ReadAllLines failed.");

    public IEnumerable<string> EnumeratePidDirectories() => throw new InvalidOperationException("EnumeratePidDirectories failed.");

    public IEnumerable<string> EnumerateFdLinks(string pidPath) => throw new InvalidOperationException("EnumerateFdLinks failed.");

    public string ReadLink(string path) => throw new InvalidOperationException("ReadLink failed.");
}
