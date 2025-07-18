namespace D20Tek.NuGet.Portfolio.Domain;

public sealed record DateRange(DateOnly Start, DateOnly End)
{
    public bool IsWithin(DateOnly date) => date >= Start && date <= End;

    public static DateRange FromWeek(DateOnly? end = null)
    {
        var endDate = end ?? DateOnlyExtensions.Today();
        return new(endDate.AddDays(-7), endDate);
    }
}
