﻿using D20Tek.NuGet.Portfolio.Features.Snapshots;

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

            bc.AddCommand<DeleteSnapshotsCommand>("delete")
              .WithAlias("del")
              .WithAlias("d")
              .WithDescription("Deletes a download snapshot for a collection for a specified date.")
              .WithExample(["snapshot", "delete", "--collection-id", "1", "--date", "7/4/2025"]);

            bc.AddCommand<ListTodayByCollectionCommand>("today")
              .WithAlias("t")
              .WithDescription("Lists the package snapshots for all packages in a collection today.")
              .WithExample(["snapshot", "today", "--collection-id", "1"]);

            bc.AddCommand<ListWeekByCollectionCommand>("week")
              .WithAlias("w")
              .WithDescription("Lists the package snapshots for all packages in a collection for the last week.")
              .WithExample(["snapshot", "week", "--collection-id", "1"]);
        })
        .WithAlias("snap")
        .WithAlias("s");
    }
}
