namespace D20Tek.NuGet.Portfolio.Common;

public static class DateOnlyExtensions
{
    public static DateOnly Today() => DateOnly.FromDateTime(DateTime.Today);
}
