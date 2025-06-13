global using D20Tek.Functional;
global using D20Tek.Functional.Async;
global using D20Tek.Spectre.Console.Extensions;
global using Spectre.Console;
global using Spectre.Console.Cli;
global using System.ComponentModel;
global using D20Tek.NuGet.Portfolio.Common;
global using D20Tek.NuGet.Portfolio.Common.Controls;

namespace D20Tek.NuGet.Portfolio;

internal static class Globals
{
    public const int S_OK = 0;

    public const int S_EXIT = unchecked((int)0x80000001);

    public const int E_FAIL = -1;

    public const string AppTitle = "NuGet Portfolio";

    public const string AppVersion = "1.0.12";

    public const string AppInitializeSuccessMsg = "How are your NuGet packages performing today?";

    public const string AppGetStartedMsg = "[green]Running interactive mode.[/] Type 'exit' to quit or '--help' to see available commands.";

    public const string AppPrompt = "nu-port>";

    public const string AppExitMessage = "Bye!";
}
