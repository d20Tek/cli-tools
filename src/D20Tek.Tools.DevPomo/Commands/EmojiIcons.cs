using D20Tek.Tools.DevPomo.Common;

namespace D20Tek.Tools.DevPomo.Commands;

internal static class EmojiIcons
{
    private static bool _emojiSupported;

    public static void Initialize() => _emojiSupported = ConsoleExtensions.SupportsEmoji();

    public static string Tomato => _emojiSupported ? ":tomato:" : "[red]*[/]";

    public static string Pause => _emojiSupported ? ":pause_button:" : "[yellow]||[/]";

    public static string Stop => _emojiSupported ? ":stop_button:" : "[red]STOP[/]";
}
