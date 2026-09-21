using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MOCTools.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "raw_epg_snapshots",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    epg_date = table.Column<DateOnly>(type: "date", nullable: false),
                    fetched_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    sha256 = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    payload = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_raw_epg_snapshots", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_raw_epg_snapshots_epg_date_fetched_at",
                table: "raw_epg_snapshots",
                columns: new[] { "epg_date", "fetched_at_utc" });

            migrationBuilder.CreateIndex(
                name: "ux_raw_epg_snapshots_epg_date_sha256",
                table: "raw_epg_snapshots",
                columns: new[] { "epg_date", "sha256" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "raw_epg_snapshots");
        }
    }
}
