using System.Diagnostics.CodeAnalysis;

namespace D20Tek.NuGet.Portfolio.Domain;

[ExcludeFromCodeCoverage]
public sealed record DateRange(DateOnly Start, DateOnly End)
{
    public bool IsWithin(DateOnly date) => date >= Start && date <= End;

    public static DateRange ForWeekEnding(DateOnly? end = null)
    {
        var endDate = end ?? DateOnlyExtensions.Today();
        return new(endDate.AddDays(-7), endDate);
    }

    public static DateRange ForMonthEnding(DateOnly? end = null)
    {
        var endDate = end ?? DateOnlyExtensions.Today();
        return new(endDate.AddMonths(-1), endDate);
    }

    public static DateRange ForYearEnding(DateOnly? end = null)
    {
        var endDate = end ?? DateOnlyExtensions.Today();
        return new(endDate.AddYears(-1), endDate);
    }
}
