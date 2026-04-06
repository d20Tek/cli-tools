using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace D20Tek.Tools.DevLog.Commands;

[ExcludeFromCodeCoverage]
internal sealed class EditablePrompt(string prompt) : IPrompt<string>
{
    private readonly string _displayPrompt = Markup.Remove(prompt.TrimStart());

    public string Show(IAnsiConsole console)
    {
        var buffer = new StringBuilder();
        var cursorPos = 0;
        console.Profile.Out.Writer.Write(_displayPrompt);

        while (true)
        {
            var key = console.Input.ReadKey(true)!.Value;

            switch (key.Key)
            {
                case ConsoleKey.Enter:
                    console.WriteLine();
                    return buffer.ToString();

                case ConsoleKey.Backspace when cursorPos > 0:
                    buffer.Remove(--cursorPos, 1);
                    break;

                case ConsoleKey.Delete when cursorPos < buffer.Length:
                    buffer.Remove(cursorPos, 1);
                    break;

                case ConsoleKey.LeftArrow when cursorPos > 0:
                    cursorPos--;
                    break;

                case ConsoleKey.RightArrow when cursorPos < buffer.Length:
                    cursorPos++;
                    break;

                case ConsoleKey.Home:
                    cursorPos = 0;
                    break;

                case ConsoleKey.End:
                    cursorPos = buffer.Length;
                    break;

                default:
                    if (!char.IsControl(key.KeyChar))
                        buffer.Insert(cursorPos++, key.KeyChar);
                    break;
            }

            Redraw(console, buffer.ToString(), cursorPos);
        }
    }

    public Task<string> ShowAsync(IAnsiConsole console, CancellationToken cancellationToken) =>
        Task.FromResult(Show(console));

    private void Redraw(IAnsiConsole console, string text, int cursorPos)
    {
        var moveBack = text.Length - cursorPos;
        var escape = moveBack > 0 ? $"\x1b[{moveBack}D" : string.Empty;
        console.Profile.Out.Writer.Write($"\r\x1b[K{_displayPrompt}{text}{escape}");
    }
}