using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MOCTools.Data.Entities;

namespace MOCTools.Data.Configurations;

public sealed class RawEpgSnapshotConfiguration
    : IEntityTypeConfiguration<RawEpgSnapshot>
{
    public void Configure(EntityTypeBuilder<RawEpgSnapshot> builder)
    {
        builder.ToTable("raw_epg_snapshots");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.EpgDate)
            .HasColumnName("epg_date")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(x => x.FetchedAtUtc)
            .HasColumnName("fetched_at_utc")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(x => x.Sha256)
            .HasColumnName("sha256")
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.Payload)
            .HasColumnName("payload")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.HasIndex(x => new { x.EpgDate, x.Sha256 })
            .IsUnique()
            .HasDatabaseName("ux_raw_epg_snapshots_epg_date_sha256");

        builder.HasIndex(x => new { x.EpgDate, x.FetchedAtUtc })
            .HasDatabaseName("ix_raw_epg_snapshots_epg_date_fetched_at");
    }
}