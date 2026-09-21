using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MOCTools.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameEpgSnapshotHashIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "ux_raw_epg_snapshots_epg_date_sha256",
                table: "raw_epg_snapshots",
                newName: "ix_raw_epg_snapshots_epg_date_sha256");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "ix_raw_epg_snapshots_epg_date_sha256",
                table: "raw_epg_snapshots",
                newName: "ux_raw_epg_snapshots_epg_date_sha256");
        }
    }
}
