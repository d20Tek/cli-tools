using D20Tek.Functional;

namespace D20Tek.Tools.DevPomo.Contracts;

public interface IConfigurationService
{
    Result<TimerConfiguration> Get();

    Result<TimerConfiguration> Set(TimerConfiguration config);
}
