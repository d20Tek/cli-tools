namespace D20Tek.Tools.DevLog.Contracts;

internal record DevLogEntry(DateOnly WeekStart, string ProjectName, List<string> Accomplishments)
{
    public static DevLogEntry Create(string projectName, List<string> accomplishments, DateOnly? date = null) =>
        new(GetWeekStart(date ?? DateOnly.FromDateTime(DateTime.Today)), projectName, accomplishments);

    public static DateOnly GetWeekStart(DateOnly date) => date.AddDays(-(int)date.DayOfWeek);
}
