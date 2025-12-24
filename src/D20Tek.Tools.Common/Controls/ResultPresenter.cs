namespace D20Tek.Tools.Common.Controls;

public static class ResultPresenter
{
    public static async Task<int> RenderAsync<T>(
        this Task<Result<T>> result,
        IAnsiConsole console,
        Func<T, string> successMessage)
        where T : notnull =>
        await result.MatchAsync(
            s => Task.FromResult(RenderSuccess(console, successMessage(s))),
            e => Task.FromResult(RenderErrors(console, e)));

    public static int Render<T>(this Result<T> result, IAnsiConsole console, Func<T, string> successMessage)
        where T : notnull =>
        result.Match(
            s => RenderSuccess(console, successMessage(s)),
            e => RenderErrors(console, e));

    private static int RenderSuccess(IAnsiConsole console, string message) =>
        console.ToIdentity().Iter(c => c.MarkupLine($"[green]Success:[/] {Markup.Escape(message)}"))
                            .Map(_ => 0);

    private static int RenderErrors(IAnsiConsole console, IEnumerable<Error> errors) =>
        errors.Count() > 1 ? RenderErrorList(console, errors) : RenderError(console, errors.First());

    private static int RenderError(IAnsiConsole console, Error error) =>
        console.ToIdentity().Iter(c => c.MarkupLine($"[red]Error[/]: {Markup.Escape(error.Message)}"))
                            .Map(_ => -1);

    private static int RenderErrorList(IAnsiConsole console, IEnumerable<Error> errors) =>
        console.ToIdentity().Iter(c => c.MarkupLine("[red]Multiple error messages[/]:"))
                            .Iter(c => errors.ForEach(e => console.MarkupLine($" - {Markup.Escape(e.Message)}")))
                            .Map(_ => -1);
}
