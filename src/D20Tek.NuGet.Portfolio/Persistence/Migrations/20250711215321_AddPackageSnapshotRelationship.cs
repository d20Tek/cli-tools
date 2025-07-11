using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace D20Tek.NuGet.Portfolio.Persistence.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class AddPackageSnapshotRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PackageSnapshots_TrackedPackages_TrackedPackageId",
                table: "PackageSnapshots");

            migrationBuilder.AddForeignKey(
                name: "FK_PackageSnapshots_TrackedPackages_TrackedPackageId",
                table: "PackageSnapshots",
                column: "TrackedPackageId",
                principalTable: "TrackedPackages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PackageSnapshots_TrackedPackages_TrackedPackageId",
                table: "PackageSnapshots");

            migrationBuilder.AddForeignKey(
                name: "FK_PackageSnapshots_TrackedPackages_TrackedPackageId",
                table: "PackageSnapshots",
                column: "TrackedPackageId",
                principalTable: "TrackedPackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
