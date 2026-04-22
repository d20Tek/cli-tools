namespace D20Tek.Tools.DevKillPort;

internal static class Constants
{
    public const string AppName = "dev-killport";
    public const string AppInitializeSuccessMsg = "Identify and kill processes bound to a port.";
    public const string AppGetStartedMsg =
        "[green]Running interactive mode.[/] Type 'exit' to quit or '--help' to see available commands.";
    public const string AppPrompt = "killport>";
    public const string AppExitMessage = "[green]Bye![/] Thanks for using dev-killport.";

    public const string PortFreeMessage = "[green]Port {0} is free.[/] No active process found.";
    public const string TimeWaitMessage =
        "[yellow]Port {0} is in TIME_WAIT state;[/] no active process to kill.";
    public const string PermissionDeniedMessage =
        "[red]Permission denied.[/] Try running with elevated privileges (sudo/admin).";
    public const string NoProcessFoundMessage = "[green]No processes found on port {0}.[/]";
    public const string KillSuccessMessage = "[green]Successfully killed process {0} (PID: {1}).[/]";
    public const string KillFailedMessage = "[red]Failed to kill process {0} (PID: {1}):[/] {2}";
    public const string DryRunMessage = "[yellow]Dry run:[/] Would kill process {0} (PID: {1}).";
    public const string ConfirmKillPrompt = "Kill this process?";
    public const string ConfirmKillAllPrompt = "Kill all {0} matching processes?";
    public const string WatchingPortMessage = "[yellow]Watching port {0}...[/] Waiting until it is free.";
    public const string WatchPortFreeMessage = "[green]Port {0} is now free.[/]";
    public const string WatchTimeoutMessage = "[red]Timeout:[/] Port {0} is still in use after {1} seconds.";
    public const string PortHeaderMessage = "Port {0} is used by:";
    public const string ListHeaderMessage = "Processes on port {0}:";
    public const string AllPortsHeaderMessage = "All ports with active processes:";
    public const string NoPortsFoundMessage = "[green]No active port bindings found.[/]";
    public const string ScanningPortsMessage = "Scanning for active ports...";

    public const string KillCommandSuccess = "Process terminated successfully.";
    public const string ListCommandSuccess = "Port scan complete.";
}
