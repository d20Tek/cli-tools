using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace D20Tek.Tools.DevLog.Commands;

[ExcludeFromCodeCoverage]
internal sealed class EditablePrompt(string prompt) : IPrompt<string>
{
    public string Show(IAnsiConsole console)
    {
        var buffer = new StringBuilder();
        var cursorPos = 0;
        console.Markup(prompt);

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
        var writer = console.Profile.Out.Writer;
        writer.Write("\r\x1b[K");
        console.Markup(prompt);
        writer.Write(text);

        var moveBack = text.Length - cursorPos;
        if (moveBack > 0) writer.Write($"\x1b[{moveBack}D");
    }
}