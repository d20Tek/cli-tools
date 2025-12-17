namespace D20Tek.Tools.DevPassword.Contracts;

internal interface IConfigurationService
{
    Result<PasswordConfig> Get();

    Result<PasswordConfig> Set(PasswordConfig config);
}
