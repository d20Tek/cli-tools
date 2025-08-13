using D20Tek.Functional;
using D20Tek.LowDb;
using D20Tek.Tools.DevPomo.Contracts;

namespace D20Tek.Tools.DevPomo.Persistence;

internal class ConfigurationService : IConfigurationService
{
    private readonly LowDb<TimerConfiguration> _db;

    public ConfigurationService(LowDb<TimerConfiguration> db) => _db = db;

    public Result<TimerConfiguration> Get() =>
        Try.Run<TimerConfiguration>(() => _db.Get());


    public Result<TimerConfiguration> Set(TimerConfiguration config)
    {
        return Try.Run<TimerConfiguration>(() =>
        {
            _db.Write();
            return config;
        });
    }
}
