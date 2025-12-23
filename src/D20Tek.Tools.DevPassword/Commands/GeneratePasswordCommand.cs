namespace D20Tek.Tools.DevPassword.Commands;

internal sealed class GeneratePasswordCommand(
    IPasswordGenerator passwordGenerator,
    IConfigurationService configurationService,
    IVerbosityWriter writer) 
    : Command<PasswordSettings>
{
    private readonly IPasswordGenerator _passwordGenerator = passwordGenerator;
    private readonly IConfigurationService _configurationService = configurationService;
    private readonly IVerbosityWriter _writer = writer;

    public override int Execute(
        CommandContext context,
        PasswordSettings settings,
        CancellationToken cancellationToken)
    {
        _writer.Verbosity = settings.Verbosity;
        _writer.WriteNormal(Constants.DevPasswordTitle);

        return _configurationService.Get()
            .Bind(config => Validate(settings, config))
            .Map(state => _passwordGenerator.Generate(state))
            .Match(r => RenderResponses(r), e => RenderErrors(e));
    }

    private static Result<PasswordState> Validate(PasswordSettings request, PasswordConfig config) =>
        ValidationErrors.Create()
            .AddIfError(
                () => request.Length < Constants.MinPasswordLength || request.Length > Constants.MaxPasswordLength,
                Constants.PasswordLengthError)
            .AddIfError(
                () => request.Count < Constants.CountMin || request.Count > Constants.CountMax, 
                Constants.PasswordCountError)
            .AddIfError(() => config.RequiredCharsAmount < 1, Constants.PasswordNoCharSetsError)
            .Map(() => new PasswordState(request.Length, request.Count, config, RndGen));

    private static int RndGen(int x) => Random.Shared.Next(x);

    private int RenderResponses(IEnumerable<PasswordResponse> responses)
    {
        responses.ForEach(response => RenderResponse(response));
        _writer.MarkupNormal(Constants.CompletionMessage);
        return 0;
    }

    private void RenderResponse(PasswordResponse response) =>
        _writer.ToIdentity()
               .Iter(w => w.MarkupSummary(Constants.PasswordMessage(response.Password.EscapeMarkup())))
               .Iter(w => w.MarkupSummary(Constants.EntropyMessage(response.Entropy)))
               .Iter(w => w.MarkupSummary(Constants.StrengthMessage(response.Strength)))
               .Iter(w => w.MarkupSummary(Constants.PasswordSeparator));

    private int RenderErrors(Error[] errors)
    {
        errors.ForEach(e => _writer.MarkupSummary($"{Constants.ErrorLabel} {e.Message}"));
        return -1;
    }
}
