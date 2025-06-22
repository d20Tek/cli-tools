using D20Tek.NuGet.Portfolio.Features.TrackedPackages;

namespace D20Tek.NuGet.Portfolio.Configuration;

internal class TrackedPackageCommandConfiguration : ICommandConfiguration
{
    public void Configure(IConfigurator config)
    {
        config.AddBranch("package", bc =>
        {
            bc.AddCommand<ListTrackedPackagesCommand>("list")
              .WithAlias("ls")
              .WithDescription("Lists all of your tracked packages.")
              .WithExample(["package", "list"]);

            bc.AddCommand<AddTrackedPackageCommand>("add")
              .WithAlias("a")
              .WithAlias("+")
              .WithDescription("Adds a new package that we are tracking.")
              .WithExample(["package", "add", "--package-id", "Test.Package.Id", "--collection-id", "1"]);

            //bc.AddCommand<EditTrackedPackageCommand>("edit")
            //  .WithAlias("ed")
            //  .WithAlias("e")
            //  .WithDescription("Edits an existing tracked package.")
            //  .WithExample(["package", "edit", "--id", "123", "--package-id", "Test.Package.Id", "--collection-id", "1"]);

            //bc.AddCommand<DeleteTrackedPackageCommand>("delete")
            //  .WithAlias("del")
            //  .WithAlias("d")
            //  .WithDescription("Deletes a tracked package by its numeric id.")
            //  .WithExample(["package", "delete", "--id", "123"]);
        })
        .WithAlias("pack")
        .WithAlias("p");
    }
}
