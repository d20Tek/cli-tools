using D20Tek.NuGet.Portfolio.Domain;
using D20Tek.Spectre.Console.Extensions.Controls;

namespace D20Tek.NuGet.Portfolio.Features.PackageDownloads;

internal class DownloadsTableBuilder
{
    private static readonly int[] _columnWidths = [5, 50, 25];
    private readonly Table _table;

    private DownloadsTableBuilder() =>
        _table = new Table().Border(TableBorder.Rounded);

    public static DownloadsTableBuilder Create() => new();

    public DownloadsTableBuilder WithHeader() =>
        _table.AddColumns(
                new TableColumn("Id").Centered().Width(5),
                new TableColumn("Package Id").Width(50),
                new TableColumn("Downloads").RightAligned().Width(25))
              .Pipe(_ => this);

    public DownloadsTableBuilder WithRows(PackageSnapshotEntity[] snapshots) =>
        (snapshots.Length == 0).ToIdentity()
                   .Iter(x => x.IfTrueOrElse(
                            () => _table.AddRow("", "No packages exist... please add some."),
                            () => snapshots.ForEach(
                                    p => _table.AddRow(
                                            p.TrackedPackageId.ToString(),
                                            p.TrackedPackage.PackageId,
                                            p.Downloads.ToString()))))
                   .Pipe(_ => this);

    public DownloadsTableBuilder WithTotals(PackageSnapshotEntity[] snapshots) =>
        snapshots.ToIdentity()
                 .Iter(_ => _table.AddSeparatorRow(_columnWidths))
                 .Iter(s => _table.AddRow("[grey]-[/]", "[grey]Total[/]", $"[grey]{s.Sum(x => x.Downloads)}[/]"))
                 .Pipe(_ => this);

    public Table Build() => _table;
}
