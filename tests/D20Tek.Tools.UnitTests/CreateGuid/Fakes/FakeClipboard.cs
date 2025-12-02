using TextCopy;

namespace D20Tek.Tools.UnitTests.CreateGuid.Fakes;

internal sealed class FakeClipboard : IClipboard
{
    private string? _clipboardText = null;

    public string? GetText() => _clipboardText;

    [ExcludeFromCodeCoverage]
    public Task<string?> GetTextAsync(CancellationToken cancellation = default) => Task.FromResult(_clipboardText);

    public void SetText(string text) => _clipboardText = text;

    [ExcludeFromCodeCoverage]
    public Task SetTextAsync(string text, CancellationToken cancellation = default)
    {
        _clipboardText = text;
        return Task.CompletedTask;
    }
}
