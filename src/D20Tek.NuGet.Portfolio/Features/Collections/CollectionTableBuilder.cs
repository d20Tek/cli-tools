using D20Tek.NuGet.Portfolio.Domain;

namespace D20Tek.NuGet.Portfolio.Features.Collections;

internal class CollectionTableBuilder
{
    private readonly Table _table;

    private CollectionTableBuilder() =>
        _table = new Table().Border(TableBorder.Rounded);

    public static CollectionTableBuilder Create() => new();

    public CollectionTableBuilder WithHeader() =>
        _table.AddColumns(
                new TableColumn("Id").Centered().Width(5),
                new TableColumn("Name").Width(50))
              .Pipe(_ => this);

    public CollectionTableBuilder WithRows(CollectionEntity[] collections) =>
        (collections.Length == 0).ToIdentity()
                   .Iter(x => x.IfTrueOrElse(
                            () => _table.AddRow("", "No collections exist... please add some."),
                            () => collections.ForEach(c => _table.AddRow(c.Id.ToString(), c.Name))))
                   .Pipe(_ => this);

    public Table Build() => _table;
}
