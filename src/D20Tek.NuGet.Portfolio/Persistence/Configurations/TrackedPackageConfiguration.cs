using D20Tek.NuGet.Portfolio.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace D20Tek.NuGet.Portfolio.Persistence.Configurations;

internal sealed class TrackedPackageConfiguration : IEntityTypeConfiguration<TrackedPackageEntity>
{
    public void Configure(EntityTypeBuilder<TrackedPackageEntity> builder)
    {
        builder.HasKey(tp => tp.Id);
        builder.Property(tp => tp.Id)
               .ValueGeneratedOnAdd();

        builder.Property(tp => tp.PackageId).IsRequired().HasMaxLength(256);

        builder.Property(tp => tp.DateAdded).IsRequired();

        builder.HasMany(tp => tp.Snapshots)
               .WithOne(ps => ps.TrackedPackage)
               .HasForeignKey(ps => ps.TrackedPackageId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
