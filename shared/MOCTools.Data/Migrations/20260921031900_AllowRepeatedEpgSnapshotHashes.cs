using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MOCTools.Data.Migrations
{
    /// <inheritdoc />
    public partial class AllowRepeatedEpgSnapshotHashes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_raw_epg_snapshots_epg_date_sha256",
                table: "raw_epg_snapshots");

            migrationBuilder.CreateIndex(
                name: "ux_raw_epg_snapshots_epg_date_sha256",
                table: "raw_epg_snapshots",
                columns: new[] { "epg_date", "sha256" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_raw_epg_snapshots_epg_date_sha256",
                table: "raw_epg_snapshots");

            migrationBuilder.CreateIndex(
                name: "ux_raw_epg_snapshots_epg_date_sha256",
                table: "raw_epg_snapshots",
                columns: new[] { "epg_date", "sha256" },
                unique: true);
        }
    }
}
