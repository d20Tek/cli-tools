namespace D20Tek.Tools.DevPassword.Services;

internal class ConfigurationService(LowDb<PasswordConfig> db) : IConfigurationService
{
    private readonly LowDb<PasswordConfig> _db = db;

    public Result<PasswordConfig> Get() => Try.Run<PasswordConfig>(() => _db.Get());


    public Result<PasswordConfig> Set(PasswordConfig updated) =>
        Try.Run<PasswordConfig>(() =>
        {
            _db.Update(current =>
            {
                current.Update(
                    updated.IncludeLowerCase,
                    updated.IncludeUpperCase,
                    updated.IncludeNumbers,
                    updated.IncludeSymbols,
                    updated.ExcludeAmbiguous,
                    updated.ExcludeBrackets);
            });
            return updated;
        });
}
