using D20Tek.Spectre.Console.Extensions.Settings;

namespace D20Tek.Tools.DevPassword.Commands;

internal sealed class UpdateConfigCommand(IAnsiConsole console, IVerbosityWriter writer, IConfigurationService service)
    : Command<VerbositySettings>
{
    private readonly IAnsiConsole _console = console;
    private readonly IVerbosityWriter _writer = writer;
    private readonly IConfigurationService _service = service;

    public override int Execute(CommandContext context, VerbositySettings settings, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(settings);
        var prevConfig = _service.Get().GetValue();

        _writer.Verbosity = settings.Verbosity;
        _writer.MarkupNormal(Constants.Configuration.Description);
        _writer.MarkupNormal();

        var incLower = _console.Confirm(Constants.Configuration.IncludeLowerLabel, prevConfig.IncludeLowerCase);
        var incUpper = _console.Confirm(Constants.Configuration.IncludeUpperLabel, prevConfig.IncludeUpperCase);
        var incNumbers = _console.Confirm(Constants.Configuration.IncludeNumbersLabel, prevConfig.IncludeNumbers);
        var incSymbols = _console.Confirm(Constants.Configuration.IncludeSymbolsLabel, prevConfig.IncludeSymbols);
        var exclAmbiguous = _console.Confirm(Constants.Configuration.ExcludeAmbiguousLabel, prevConfig.ExcludeAmbiguous);
        var exclBrackets = _console.Confirm(Constants.Configuration.ExcludeBracketsLabel, prevConfig.ExcludeBrackets);

        var result = _service.Set(
            new PasswordConfig(incLower, incUpper, incNumbers, incSymbols, exclAmbiguous, exclBrackets));

        return result.Match(_ => _writer.RenderCompletion(Constants.CompletionMessage), e => _writer.RenderErrors(e));
    }
}
