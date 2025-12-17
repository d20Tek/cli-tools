namespace D20Tek.Tools.DevPassword.Contracts;

internal sealed class PasswordConfig
{
    public bool IncludeLowerCase { get; set; } = true;

    public bool IncludeUpperCase { get; set; } = true;

    public bool IncludeNumbers { get; set; } = true;

    public bool IncludeSymbols { get; set; } = true;

    public bool ExcludeAmbiguous { get; set; } = false;

    public bool ExcludeBrackets { get; set; } = false;

    public int RequiredCharsAmount =>
        IncludeLowerCase.ToInt() + IncludeUpperCase.ToInt() + IncludeNumbers.ToInt() + IncludeSymbols.ToInt();

    public PasswordConfig(
        bool includeLowerCase = true,
        bool includeUpperCase = true,
        bool includeNumbers = true,
        bool includeSymbols = true,
        bool excludeAmbiguous = false,
        bool excludeBrackets = false)
    {
        IncludeLowerCase = includeLowerCase;
        IncludeUpperCase = includeUpperCase;
        IncludeNumbers = includeNumbers;
        IncludeSymbols = includeSymbols;
        ExcludeAmbiguous = excludeAmbiguous;
        ExcludeBrackets = excludeBrackets;
    }

    public PasswordConfig() { }

    public void Update(
        bool includeLowerCase,
        bool includeUpperCase,
        bool includeNumbers,
        bool includeSymbols,
        bool excludeAmbiguous,
        bool excludeBrackets)
    {
        IncludeLowerCase = includeLowerCase;
        IncludeUpperCase = includeUpperCase;
        IncludeNumbers = includeNumbers;
        IncludeSymbols = includeSymbols;
        ExcludeAmbiguous = excludeAmbiguous;
        ExcludeBrackets = excludeBrackets;
    }
};
