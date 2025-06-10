using D20Tek.NuGet.Portfolio.Common.Controls;

namespace D20Tek.NuGet.Portfolio.Common;

internal static class AnsiConsoleExtensions
{
    public static CommandHeader CommandHeader(this IAnsiConsole console) => new(console);
}
