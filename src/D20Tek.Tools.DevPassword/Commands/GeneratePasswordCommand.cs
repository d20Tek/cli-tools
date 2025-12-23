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
        _writer.WriteNormal("dev-password: generating password...");

        return _configurationService.Get()
            .Bind(config => Validate(settings, config))
            .Map(state => _passwordGenerator.Generate(state))
            .Match(r => RenderResponse(r), e => RenderErrors(e));
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
            .Map(() => new PasswordState(request.Length, config, RndGen));

    private static int RndGen(int x) => Random.Shared.Next(x);

    private int RenderResponse(PasswordResponse response) =>
        _writer.ToIdentity()
               .Iter(w => w.MarkupSummary(Constants.PasswordMessage(response.Password.EscapeMarkup())))
               .Iter(w => w.MarkupSummary(Constants.EntropyMessage(response.Entropy)))
               .Iter(w => w.MarkupSummary(Constants.StrengthMessage(response.Strength)))
               .Iter(w => w.MarkupNormal("[green]Command completed successfully![/]"))
               .Pipe(_ => 0);

    private int RenderErrors(Error[] errors)
    {
        errors.ForEach(e => _writer.MarkupSummary($"[red]Error:[/] {e.Message}"));
        return -1;
    }
}
