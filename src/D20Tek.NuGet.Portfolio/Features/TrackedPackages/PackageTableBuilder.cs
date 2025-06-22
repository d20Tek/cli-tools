using D20Tek.NuGet.Portfolio.Domain;

namespace D20Tek.NuGet.Portfolio.Features.TrackedPackages;

internal class PackageTableBuilder
{
    private readonly Table _table;

    private PackageTableBuilder() =>
        _table = new Table().Border(TableBorder.Rounded);

    public static PackageTableBuilder Create() => new();

    public PackageTableBuilder WithHeader() =>
        _table.AddColumns(
                new TableColumn("Id").Centered().Width(5),
                new TableColumn("Package Id").Width(50),
                new TableColumn("Collection").Width(25))
              .Pipe(_ => this);

    public PackageTableBuilder WithRows(TrackedPackageEntity[] packages) =>
        (packages.Length == 0).ToIdentity()
                   .Iter(x => x.IfTrueOrElse(
                            () => _table.AddRow("", "No packages exist... please add some."),
                            () => packages.ForEach(
                                    p => _table.AddRow(p.Id.ToString(), p.PackageId, p.Collection.Name))))
                   .Pipe(_ => this);

    public Table Build() => _table;
}
