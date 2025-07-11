using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace D20Tek.NuGet.Portfolio.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPackageSnapshotDateIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PackageSnapshot_SnapshotDate",
                table: "PackageSnapshots",
                column: "SnapshotDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PackageSnapshot_SnapshotDate",
                table: "PackageSnapshots");
        }
    }
}
