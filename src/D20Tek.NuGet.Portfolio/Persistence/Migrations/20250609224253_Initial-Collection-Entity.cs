using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace D20Tek.NuGet.Portfolio.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCollectionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Collections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrackedPackages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PackageId = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    DateAdded = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CollectionId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackedPackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackedPackages_Collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PackageSnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SnapshotDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Downloads = table.Column<int>(type: "INTEGER", nullable: false),
                    TrackedPackageId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackageSnapshots_TrackedPackages_TrackedPackageId",
                        column: x => x.TrackedPackageId,
                        principalTable: "TrackedPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PackageSnapshots_TrackedPackageId",
                table: "PackageSnapshots",
                column: "TrackedPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackedPackages_CollectionId",
                table: "TrackedPackages",
                column: "CollectionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PackageSnapshots");

            migrationBuilder.DropTable(
                name: "TrackedPackages");

            migrationBuilder.DropTable(
                name: "Collections");
        }
    }
}
