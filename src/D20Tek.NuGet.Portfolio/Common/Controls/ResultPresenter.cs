namespace D20Tek.NuGet.Portfolio.Common.Controls;

internal static class ResultPresenter
{
    public static async Task<int> RenderAsync<T>(this Task<Result<T>> result, IAnsiConsole console, Func<T, string> successMessage)
        where T : notnull =>
        await result.MatchAsync(
            s => Task.FromResult(RenderSuccess(console, successMessage(s))),
            e => Task.FromResult(RenderErrors(console, e)));

    public static int Render<T>(this Result<T> result, IAnsiConsole console, Func<T, string> successMessage)
        where T : notnull =>
        result.Match(
            s => RenderSuccess(console, successMessage(s)),
            e => RenderErrors(console, e));

    private static int RenderSuccess(IAnsiConsole console, string message)
    {
        console.MarkupLine($"[green]Success:[/] {Markup.Escape(message)}");
        return Globals.S_OK;
    }

    private static int RenderErrors(IAnsiConsole console, IEnumerable<Error> errors)
    {
        if (errors.Count() > 1)
        {
            console.MarkupLine("[red]Multiple error messages[/]:");
            errors.ForEach(e => console.MarkupLine($" - {Markup.Escape(e.Message)}"));
        }
        else if (errors.Count() == 1)
        {
            RenderError(console, errors.First());
        }

        return Globals.E_FAIL;
    }

    private static void RenderError(IAnsiConsole console, Error error) =>
        console.MarkupLine($"[red]Error[/]: {Markup.Escape(error.Message)}");
}
