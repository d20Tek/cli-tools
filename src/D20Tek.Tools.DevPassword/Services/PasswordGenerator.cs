namespace D20Tek.Tools.DevPassword.Services;

internal class PasswordGenerator : IPasswordGenerator
{
    public PasswordResponse Generate(PasswordState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentOutOfRangeException.ThrowIfLessThan(state.Length, Constants.MinPasswordLength);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(state.Length, Constants.MaxPasswordLength);
        ArgumentNullException.ThrowIfNull(state.Config);
        ArgumentNullException.ThrowIfNull(state.Rnd);


        return state.Map(GetCharacterSet)
                    .Map(CalculateEntropy)
                    .Map(s => new PasswordResponse(
                         Shuffle(GetRandomCharacters(s), s.Rnd),
                         s.Entropy,
                         Constants.DetermineStrength(s.Entropy)));
    }

    private static PasswordState GetCharacterSet(PasswordState s) => s with { CharSet = s.Config.GetCharacterSet() };
    
    private static string GetRandomCharacters(PasswordState state) => new(
        Enumerable.Range(0, state.Length - state.Config.RequiredCharsAmount)
                  .Select(_ => state.CharSet.GetRandomCharacter(state.Rnd))
                  .Concat(state.Config.GetRequiredCharacters(state.Rnd))
                  .ToArray());

    private static string Shuffle(string inputChars, Func<int, int> rnd) =>
        new([.. inputChars.ToCharArray().OrderBy(_ => rnd(int.MaxValue))]);

    private static PasswordState CalculateEntropy(PasswordState state) =>
        state with { Entropy = state.Length * Math.Log2(state.CharSet.Length) };
}
