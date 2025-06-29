namespace D20Tek.NuGet.Portfolio.Common;

public static class TableExtensions
{
    public static void AddSeparatorRow(this Table table, int[] columnWidths, string style = "grey", char separatorChar = '─') =>
        Enumerable.Range(0, columnWidths.Length)
                  .Select(i => new Markup($"[{style}]{new string(separatorChar, columnWidths[i])}[/]"))
                  .ToArray()
                  .Pipe(row => table.AddRow(row));
}
