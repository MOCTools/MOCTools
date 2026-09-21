using System.IO;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MOCTools.Data;
using MOCTools.EpgFetcher;

var builder = Host.CreateApplicationBuilder(args);

var envFile = Path.Combine(
    builder.Environment.ContentRootPath,
    ".env");

if (File.Exists(envFile))
{
    Env.NoClobber().Load(envFile);
}

var connectionString =
    Environment.GetEnvironmentVariable("MOCTOOLS_DB_CONNECTION_STRING")
    ?? throw new InvalidOperationException(
        "Environment variable MOCTOOLS_DB_CONNECTION_STRING is not set.");

builder.Services.AddHttpClient();

builder.Services.AddDbContextFactory<MOCToolsDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();