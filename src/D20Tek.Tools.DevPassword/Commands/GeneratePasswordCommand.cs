using D20Tek.Spectre.Console.Extensions.Services;

namespace D20Tek.Tools.DevPassword.Commands;

internal sealed class GeneratePasswordCommand(IVerbosityWriter writer) : Command<PasswordSettings>
{
    private readonly IVerbosityWriter _writer = writer;

    public override int Execute(
        CommandContext context,
        PasswordSettings settings,
        CancellationToken cancellationToken)
    {
        _writer.Verbosity = settings.Verbosity;
        _writer.WriteNormal("dev-password: generating password...");

        _writer.MarkupNormal("[green]Command completed successfully![/]");
        return 0;
    }
}
