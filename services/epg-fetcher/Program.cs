using DotNetEnv;
using EpgFetcher;

var builder = Host.CreateApplicationBuilder(args);

var envFile = Path.Combine(
    builder.Environment.ContentRootPath,
    ".env");

if (File.Exists(envFile))
{
    Env.NoClobber().Load(envFile);
}

builder.Services.AddHttpClient();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
