using System.Diagnostics.CodeAnalysis;

namespace D20Tek.Tools.Common.Controls;

public static class ResultPresenter
{
    private const string SuccessLabel = "[green]Success:[/]";
    private const string ErrorLabel = "[red]Error:[/]";
    private const string MultipleErrorsLabel = "[red]Multiple error messages[/]:";

    public static async Task<int> RenderAsync<T>(
        this Task<Result<T>> result,
        IAnsiConsole console,
        Func<T, string> successMessage)
        where T : notnull =>
        await result.MatchAsync(
            s => Task.FromResult(RenderSuccess(console.MarkupLine, successMessage(s))),
            e => Task.FromResult(RenderErrors(console.MarkupLine, e)));

    public static int Render<T>(this Result<T> result, IAnsiConsole console, Func<T, string> successMessage)
        where T : notnull =>
        result.Match(
            s => RenderSuccess(console.MarkupLine, successMessage(s)),
            e => RenderErrors(console.MarkupLine, e));

    /* Removing RenderAsync until AsyncCommands are used.
    public static async Task<int> RenderAsync<T>(
        this Task<Result<T>> result,
        IVerbosityWriter writer,
        Func<T, string> successMessage)
        where T : notnull =>
        await result.MatchAsync(
            s => Task.FromResult(RenderSuccess(writer.MarkupNormal, successMessage(s))),
            e => Task.FromResult(RenderErrors(writer.MarkupSummary, e)));
    */

    public static int Render<T>(this Result<T> result, IVerbosityWriter writer, Func<T, string> successMessage)
        where T : notnull =>
        result.Match(
            s => RenderSuccess(writer.MarkupNormal, successMessage(s)),
            e => RenderErrors(writer.MarkupSummary, e));

    private static int RenderSuccess(Action<string> renderMarkup, string message) =>
        renderMarkup.ToIdentity().Iter(r => r($"{SuccessLabel} {Markup.Escape(message)}"))
                                 .Map(_ => 0);

    private static int RenderErrors(Action<string> renderMarkup, IEnumerable<Error> errors) =>
        errors.Count() > 1 ? RenderErrorList(renderMarkup, errors) : RenderError(renderMarkup, errors.First());

    private static int RenderError(Action<string> renderMarkup, Error error) =>
        renderMarkup.ToIdentity().Iter(r => r($"{ErrorLabel} {Markup.Escape(error.Message)}"))
                                 .Map(_ => -1);

    private static int RenderErrorList(Action<string> renderMarkup, IEnumerable<Error> errors) =>
        renderMarkup.ToIdentity().Iter(r => r(MultipleErrorsLabel))
                                 .Iter(r => errors.ForEach(e => r($" - {Markup.Escape(e.Message)}")))
                                 .Map(_ => -1);
}
