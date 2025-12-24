namespace D20Tek.Tools.Common;

public static class DateOnlyExtensions
{
    public static DateOnly Today() => DateOnly.FromDateTime(DateTime.Today);
}
