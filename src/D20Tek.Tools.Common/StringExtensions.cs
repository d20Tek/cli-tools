namespace D20Tek.Tools.Common;

public static class StringExtensions
{
    public static DateOnly? ParseDate(this string dateString) =>
        DateOnly.TryParse(dateString, out var date) ? date : null;
}
