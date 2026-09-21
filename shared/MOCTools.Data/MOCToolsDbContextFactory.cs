using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MOCTools.Data;

public sealed class MOCToolsDbContextFactory
    : IDesignTimeDbContextFactory<MOCToolsDbContext>
{
    public MOCToolsDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("MOCTOOLS_DB_CONNECTION_STRING")
            ?? throw new InvalidOperationException(
                "Environment variable MOCTOOLS_DB_CONNECTION_STRING is not set.");

        var options = new DbContextOptionsBuilder<MOCToolsDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new MOCToolsDbContext(options);
    }
}