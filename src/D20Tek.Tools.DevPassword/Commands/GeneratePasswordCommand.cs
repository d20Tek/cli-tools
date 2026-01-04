using D20Tek.Tools.Common.Controls;

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
        _writer.RenderCommandTitle(Constants.DevPasswordTitle, settings.Verbosity);

        return _configurationService.Get()
            .Bind(config => Validate(settings, config))
            .Map(state => _passwordGenerator.Generate(state))
            .Render(_writer, RenderResponses);
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

    private string RenderResponses(IEnumerable<PasswordResponse> responses) =>
        responses.ToIdentity()
                 .Iter(r => RenderResponseMetadata(r.First()))
                 .Iter(_ => _writer.MarkupNormal(Constants.PasswordSeparator))
                 .Iter(r => RenderPasswords(r))
                 .Iter(_ => _writer.MarkupNormal(Constants.PasswordSeparator))
                 .Map(_ => Constants.CompletionMessage);

    private void RenderResponseMetadata(PasswordResponse response) =>
        _writer.MarkupNormal(
            $"{Constants.LengthMessage(response.Password.Length)} | " +
            $"{Constants.EntropyMessage(response.Entropy)} | " +
            $"{Constants.StrengthMessage(response.Strength)}");

    private void RenderPasswords(IEnumerable<PasswordResponse> responses) =>
        responses.ForEach(response => _writer.MarkupSummary(response.Password.EscapeMarkup()));
}
