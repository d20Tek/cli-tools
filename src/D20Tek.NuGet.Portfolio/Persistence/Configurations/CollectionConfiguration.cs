using D20Tek.NuGet.Portfolio.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace D20Tek.NuGet.Portfolio.Persistence.Configurations;

internal class CollectionConfiguration : IEntityTypeConfiguration<CollectionEntity>
{
    public void Configure(EntityTypeBuilder<CollectionEntity> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
               .ValueGeneratedOnAdd();

        builder.Property(c => c.Name)
               .IsRequired()
               .HasMaxLength(32);

        builder.HasMany(c => c.TrackedPackages)
               .WithOne(tp => tp.Collection)
               .HasForeignKey(tp => tp.CollectionId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
