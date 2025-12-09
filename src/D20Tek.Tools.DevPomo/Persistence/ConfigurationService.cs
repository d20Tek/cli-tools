namespace D20Tek.Tools.DevPomo.Persistence;

internal class ConfigurationService(LowDb<TimerConfiguration> db) : IConfigurationService
{
    private readonly LowDb<TimerConfiguration> _db = db;

    public Result<TimerConfiguration> Get() => Try.Run<TimerConfiguration>(() => _db.Get());


    public Result<TimerConfiguration> Set(TimerConfiguration updated) =>
        Try.Run<TimerConfiguration>(() =>
        {
            _db.Update(current =>
            {
                current.Update(
                    updated.PomodoroMinutes,
                    updated.BreakMinutes,
                    updated.ShowAppTitleBar,
                    updated.EnableSound,
                    updated.AutostartCycles,
                    updated.MinimalOutput);
            });
            return updated;
        });
}
