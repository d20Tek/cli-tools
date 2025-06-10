namespace D20Tek.NuGet.Portfolio.Common.Controls;

internal static class ResultPresenter
{
    public static async Task<int> RenderAsync<T>(this Task<Result<T>> result, IAnsiConsole console, Func<T, string> successMessage)
        where T : notnull =>
        await result.MatchAsync(
            s => Task.FromResult(RenderSuccess(console, successMessage(s))),
            e => Task.FromResult(RenderErrors(console, e)));

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
            foreach (var error in errors)
            {
                var msg = Markup.Escape(error.Message);
                console.MarkupLine($" - {msg}");
            }
        }
        else if (errors.Count() == 1)
        {
            RenderError(errors.First(), console);
        }

        return Globals.E_FAIL;
    }

    private static int RenderError(Error error, IAnsiConsole console)
    {
        var msg = Markup.Escape(error.Message);
        console.MarkupLine($"[red]Error[/]: {msg}");
        return Globals.E_FAIL;
    }
}
