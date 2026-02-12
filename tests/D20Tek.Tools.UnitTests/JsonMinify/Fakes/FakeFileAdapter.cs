using D20Tek.Tools.JsonMinify.Services;

namespace D20Tek.Tools.UnitTests.JsonMinify.Fakes;

internal sealed class FakeFileAdapter(string fileBody) : IFileAdapter
{
    public bool Exists(string path) => string.IsNullOrEmpty(fileBody) is false;

    public string ReadAllText(string path) => fileBody;

    public void WriteAllText(string path, string contents) { }
}
