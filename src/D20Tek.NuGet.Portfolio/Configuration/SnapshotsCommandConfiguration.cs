using D20Tek.NuGet.Portfolio.Features.Snapshots;

namespace D20Tek.NuGet.Portfolio.Configuration;

internal sealed class SnapshotsCommandConfiguration : ICommandConfiguration
{
    public void Configure(IConfigurator config)
    {
        config.AddBranch("snapshot", bc =>
        {
            bc.AddCommand<AddSnapshotCommand>("add")
              .WithAlias("a")
              .WithAlias("+")
              .WithDescription("Saves the download snapshot today for the specified collection id.")
              .WithExample(["snapshot", "add", "--collection-id", "1"]);
        })
        .WithAlias("snap")
        .WithAlias("s");
    }
}
