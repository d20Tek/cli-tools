using D20Tek.Tools.DevLog.Contracts;

namespace D20Tek.Tools.DevLog.Services;

internal static class MarkdownSerializer
{
    internal static List<DevLogEntry> ParseEntries(string content, DateOnly weekStart)
    {
        var entries = new List<DevLogEntry>();
        string? currentProject = null;
        var currentAccomplishments = new List<string>();

        foreach (var line in content.Split('\n'))
        {
            if (line.StartsWith("### "))
            {
                if (currentProject is not null)
                    entries.Add(new DevLogEntry(weekStart, currentProject, currentAccomplishments));

                currentProject = line[4..].TrimEnd();
                currentAccomplishments = [];
            }
            else if (currentProject is not null && line.StartsWith(Constants.AccomplishmentBullet))
            {
                currentAccomplishments.Add(line[Constants.AccomplishmentBullet.Length..].TrimEnd());
            }
        }

        if (currentProject is not null)
            entries.Add(new DevLogEntry(weekStart, currentProject, currentAccomplishments));

        return entries;
    }

    internal static string SerializeEntries(DateOnly weekStart, List<DevLogEntry> entries) =>
        $"{Constants.WeekHeader(weekStart)}\n\n" +
        string.Join("\n\n", entries.Select(GenerateProjectSection));

    private static string GenerateProjectSection(DevLogEntry entry) =>
        $"{Constants.ProjectHeader(entry.ProjectName)}\n" +
        string.Join("\n", entry.Accomplishments.Select(a => $"{Constants.AccomplishmentBullet}{a}"));
}
