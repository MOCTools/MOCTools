using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MOCTools.Data;
using MOCTools.Data.Entities;

namespace MOCTools.EpgFetcher;

public class Worker(
    ILogger<Worker> logger,
    IHttpClientFactory httpClientFactory,
    IDbContextFactory<MOCToolsDbContext> dbContextFactory)
    : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var timeZone =
            TimeZoneInfo.FindSystemTimeZoneById("Europe/Amsterdam");

        var urlTemplate =
            Environment.GetEnvironmentVariable("EPG_URL_TEMPLATE")
            ?? throw new InvalidOperationException(
                "EPG_URL_TEMPLATE is not configured.");

        var client = httpClientFactory.CreateClient();

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = TimeZoneInfo.ConvertTime(
                DateTimeOffset.UtcNow,
                timeZone);

            logger.LogInformation(
                "Starting EPG fetch cycle at {Time}",
                now);

            for (var dayOffset = 0; dayOffset < 7; dayOffset++)
            {
                var epgDate = now.AddDays(dayOffset);

                await FetchEpgForDateAsync(
                    client,
                    urlTemplate,
                    epgDate,
                    stoppingToken);
            }

            logger.LogInformation(
                "EPG fetch cycle completed. Next run in 1 hour.");

            await Task.Delay(
                TimeSpan.FromHours(1),
                stoppingToken);
        }
    }

    private async Task FetchEpgForDateAsync(
        HttpClient client,
        string urlTemplate,
        DateTimeOffset epgDate,
        CancellationToken stoppingToken)
    {
        var date = epgDate.ToString("yyyy-MM-dd");
        var url = urlTemplate.Replace("{date}", date);

        try
        {
            using var response = await client.GetAsync(
                url,
                stoppingToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "EPG request for {Date} failed with HTTP {StatusCode}",
                    date,
                    (int)response.StatusCode);

                return;
            }

            var content = await response.Content.ReadAsStringAsync(
                stoppingToken);

            if (!IsValidJson(content, date))
            {
                return;
            }

            var payloadHash = CalculateHash(content);

            logger.LogInformation(
                "Successfully fetched EPG data for {Date} with hash {Hash}",
                date,
                payloadHash);

            await SaveSnapshotAsync(
                DateOnly.FromDateTime(epgDate.DateTime),
                payloadHash,
                content,
                stoppingToken);
        }
        catch (HttpRequestException exception)
        {
            logger.LogError(
                exception,
                "EPG request for {Date} failed with exception",
                date);
        }
    }

    private async Task SaveSnapshotAsync(
        DateOnly epgDate,
        string sha256,
        string payload,
        CancellationToken stoppingToken)
    {
        await using var dbContext =
            await dbContextFactory.CreateDbContextAsync(stoppingToken);

        var latestHash = await dbContext.RawEpgSnapshots
            .Where(snapshot => snapshot.EpgDate == epgDate)
            .OrderByDescending(snapshot => snapshot.FetchedAtUtc)
            .Select(snapshot => snapshot.Sha256)
            .FirstOrDefaultAsync(stoppingToken);

        if (latestHash == sha256)
        {
            logger.LogInformation(
                "EPG data for {Date} has not changed; snapshot not stored",
                epgDate);

            return;
        }

        var snapshot = new RawEpgSnapshot
        {
            EpgDate = epgDate,
            FetchedAtUtc = DateTimeOffset.UtcNow,
            Sha256 = sha256,
            Payload = payload
        };

        dbContext.RawEpgSnapshots.Add(snapshot);

        await dbContext.SaveChangesAsync(stoppingToken);

        logger.LogInformation(
            "Stored new EPG snapshot for {Date} with hash {Hash}",
            epgDate,
            sha256);
    }

    private bool IsValidJson(
        string content,
        string date)
    {
        try
        {
            using var document = JsonDocument.Parse(content);

            logger.LogInformation(
                "EPG response for {Date} contains valid JSON",
                date);

            return true;
        }
        catch (JsonException exception)
        {
            logger.LogError(
                exception,
                "EPG response for {Date} contains invalid JSON",
                date);

            return false;
        }
    }

    private static string CalculateHash(string content)
    {
        var hashBytes = SHA256.HashData(
            Encoding.UTF8.GetBytes(content));

        return Convert.ToHexString(hashBytes);
    }
}