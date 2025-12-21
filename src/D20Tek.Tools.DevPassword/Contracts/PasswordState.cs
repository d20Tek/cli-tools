namespace D20Tek.Tools.DevPassword.Contracts;

internal sealed record PasswordState(
    int Length,
    PasswordConfig Config,
    Func<int, int> Rnd,
    string CharSet = "",
    double Entropy = 0) : IState;
