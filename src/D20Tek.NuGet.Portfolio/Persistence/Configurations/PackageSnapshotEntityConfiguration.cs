using D20Tek.NuGet.Portfolio.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace D20Tek.NuGet.Portfolio.Persistence.Configurations;

internal sealed class PackageSnapshotConfiguration : IEntityTypeConfiguration<PackageSnapshotEntity>
{
    public void Configure(EntityTypeBuilder<PackageSnapshotEntity> builder)
    {
        builder.HasKey(ps => ps.Id);
        builder.Property(ps => ps.Id)
               .ValueGeneratedOnAdd();


        builder.Property(ps => ps.SnapshotDate).IsRequired();
        
        builder.Property(ps => ps.Downloads).IsRequired();

        builder.HasOne(ps => ps.TrackedPackage)
               .WithMany(tp => tp.Snapshots)
               .HasForeignKey(ps => ps.TrackedPackageId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}