using D20Tek.NuGet.Portfolio.Features.PackageDownloads;

namespace D20Tek.NuGet.Portfolio.Configuration;

internal class PackageDownloadsConfiguration : ICommandConfiguration
{
    public void Configure(IConfigurator config)
    {
        config.AddBranch("get-current", bc =>
        {
            bc.AddCommand<GetDownloadsByPackageIdCommand>("for-id")
              .WithAlias("pid")
              .WithDescription("Gets the current download count for the specified package id.")
              .WithExample(["get-current", "for-id", "--id", "5"]);
        })
        .WithAlias("get-curr")
        .WithAlias("curr")
        .WithAlias("gc");
    }
}
