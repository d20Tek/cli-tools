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
        _writer.RenderCommandTitle(Constants.Configuration.Description, settings.Verbosity);

        return _service.Get()
                       .Bind(prevConfig => _service.Set(CollectConfigInput(prevConfig)))
                       .Match(_ => _writer.RenderCompletion(Constants.CompletionMessage), _writer.RenderErrors);
    }

    private PasswordConfig CollectConfigInput(PasswordConfig prevConfig)
    {
        var incLower = _console.Confirm(Constants.Configuration.IncludeLowerLabel, prevConfig.IncludeLowerCase);
        var incUpper = _console.Confirm(Constants.Configuration.IncludeUpperLabel, prevConfig.IncludeUpperCase);
        var incNumbers = _console.Confirm(Constants.Configuration.IncludeNumbersLabel, prevConfig.IncludeNumbers);
        var incSymbols = _console.Confirm(Constants.Configuration.IncludeSymbolsLabel, prevConfig.IncludeSymbols);
        var exclAmbiguous = _console.Confirm(Constants.Configuration.ExcludeAmbiguousLabel, prevConfig.ExcludeAmbiguous);
        var exclBrackets = _console.Confirm(Constants.Configuration.ExcludeBracketsLabel, prevConfig.ExcludeBrackets);

        return new(incLower, incUpper, incNumbers, incSymbols, exclAmbiguous, exclBrackets);
    }
}
