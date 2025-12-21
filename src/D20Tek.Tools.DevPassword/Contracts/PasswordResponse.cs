namespace D20Tek.Tools.DevPassword.Contracts;

internal sealed record PasswordResponse(string Password, double Entropy, string Strength);
