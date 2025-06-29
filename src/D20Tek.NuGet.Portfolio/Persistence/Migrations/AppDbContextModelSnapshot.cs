﻿// <auto-generated />
using System;
using System.Diagnostics.CodeAnalysis;
using D20Tek.NuGet.Portfolio.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace D20Tek.NuGet.Portfolio.Persistence.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [ExcludeFromCodeCoverage]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.5");

            modelBuilder.Entity("D20Tek.NuGet.Portfolio.Domain.CollectionEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Collections");
                });

            modelBuilder.Entity("D20Tek.NuGet.Portfolio.Domain.PackageSnapshotEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Downloads")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("SnapshotDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("TrackedPackageId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("TrackedPackageId");

                    b.ToTable("PackageSnapshots");
                });

            modelBuilder.Entity("D20Tek.NuGet.Portfolio.Domain.TrackedPackageEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CollectionId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("TEXT");

                    b.Property<string>("PackageId")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CollectionId");

                    b.ToTable("TrackedPackages");
                });

            modelBuilder.Entity("D20Tek.NuGet.Portfolio.Domain.PackageSnapshotEntity", b =>
                {
                    b.HasOne("D20Tek.NuGet.Portfolio.Domain.TrackedPackageEntity", "TrackedPackage")
                        .WithMany("Snapshots")
                        .HasForeignKey("TrackedPackageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TrackedPackage");
                });

            modelBuilder.Entity("D20Tek.NuGet.Portfolio.Domain.TrackedPackageEntity", b =>
                {
                    b.HasOne("D20Tek.NuGet.Portfolio.Domain.CollectionEntity", "Collection")
                        .WithMany("TrackedPackages")
                        .HasForeignKey("CollectionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Collection");
                });

            modelBuilder.Entity("D20Tek.NuGet.Portfolio.Domain.CollectionEntity", b =>
                {
                    b.Navigation("TrackedPackages");
                });

            modelBuilder.Entity("D20Tek.NuGet.Portfolio.Domain.TrackedPackageEntity", b =>
                {
                    b.Navigation("Snapshots");
                });
#pragma warning restore 612, 618
        }
    }
}
