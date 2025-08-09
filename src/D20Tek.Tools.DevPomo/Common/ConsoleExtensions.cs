namespace D20Tek.Tools.DevPomo.Common;

public static class ConsoleExtensions
{
    /// <summary>
    /// Check if the conosle can render a known emoji character.
    /// </summary>
    public static bool SupportsEmoji()
    {
        try
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            string test = "🍅";
            return test.Any(
                c => char.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.OtherNotAssigned);
        }
        catch
        {
            return false;
        }
    }
}
