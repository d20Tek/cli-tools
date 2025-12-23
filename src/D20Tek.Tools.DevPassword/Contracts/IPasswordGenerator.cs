namespace D20Tek.Tools.DevPassword.Contracts;

internal interface IPasswordGenerator
{
    IEnumerable<PasswordResponse> Generate(PasswordState state);
}
