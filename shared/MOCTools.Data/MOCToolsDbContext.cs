using Microsoft.EntityFrameworkCore;
using MOCTools.Data.Entities;

namespace MOCTools.Data;

public sealed class MOCToolsDbContext(
    DbContextOptions<MOCToolsDbContext> options)
    : DbContext(options)
{
    public DbSet<RawEpgSnapshot> RawEpgSnapshots =>
        Set<RawEpgSnapshot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(MOCToolsDbContext).Assembly);
    }
}