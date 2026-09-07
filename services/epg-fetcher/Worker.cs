using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace EpgFetcher;

public class Worker(
    ILogger<Worker> logger,
    IHttpClientFactory httpClientFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var timeZone =
            TimeZoneInfo.FindSystemTimeZoneById("Europe/Amsterdam");

        var now = TimeZoneInfo.ConvertTime(
            DateTimeOffset.UtcNow,
            timeZone);

        var urlTemplate =
            Environment.GetEnvironmentVariable("EPG_URL_TEMPLATE")
            ?? throw new InvalidOperationException(
                "EPG_URL_TEMPLATE is not configured.");

        var client = httpClientFactory.CreateClient();

        for (var dayOffset = 0; dayOffset < 7; dayOffset++)
        {
            var epgDate = now.AddDays(dayOffset);

            await FetchEpgForDateAsync(
                client,
                urlTemplate,
                epgDate,
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
        }
        catch (HttpRequestException exception)
        {
            logger.LogError(
                exception,
                "EPG request for {Date} failed with exception",
                date);
        }
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