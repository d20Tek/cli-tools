﻿using D20Tek.NuGet.Portfolio.Features.Collections;

namespace D20Tek.NuGet.Portfolio.Configuration;

internal sealed class CollectionCommandConfiguration : ICommandConfiguration
{
    public void Configure(IConfigurator config)
    {
        config.AddBranch("collection", bc =>
        {
            bc.AddCommand<ListCollectionsCommand>("list")
              .WithAlias("ls")
              .WithDescription("Lists all of your package collections.")
              .WithExample(["collection", "list"]);

            bc.AddCommand<AddCollectionCommand>("add")
              .WithAlias("a")
              .WithAlias("+")
              .WithDescription("Adds a new package collection that can be used for organization and tracking.")
              .WithExample(["collection", "add", "--name", "New Collection"]);

            bc.AddCommand<EditCollectionCommand>("edit")
              .WithAlias("ed")
              .WithAlias("e")
              .WithDescription("Edits an existing package collection.")
              .WithExample(["collection", "edit", "--id", "123", "--name", "New Collection"]);

            bc.AddCommand<DeleteCollectionCommand>("delete")
              .WithAlias("del")
              .WithAlias("d")
              .WithDescription("Deletes a package collection by its id.")
              .WithExample(["collection", "delete", "--id", "123"]);
        })
        .WithAlias("coll")
        .WithAlias("c");
    }
}
