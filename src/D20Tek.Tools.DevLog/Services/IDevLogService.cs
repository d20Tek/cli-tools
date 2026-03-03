namespace D20Tek.Tools.DevLog.Services;

internal interface IDevLogService
{
    Result<bool> AddEntry(string logFolder, string projectName, List<string> accomplishments, DateOnly? date = null);

    Result<string> ViewLog(string logFolder, DateOnly? date = null);

    Result<bool> EditEntry(string logFolder, string projectName, List<string> accomplishments, DateOnly? date = null);

    Result<List<string>> GetAccomplishments(string logFolder, string projectName, DateOnly? date = null);

    Result<IEnumerable<string>> ListLogs(string logFolder);
}
