namespace D20Tek.Tools.DevPassword.Contracts;

internal interface IPasswordGenerator
{
    PasswordResponse Generate(PasswordState state);
}
