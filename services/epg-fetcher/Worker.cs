using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

namespace EpgFetcher;

public class Worker(ILogger<Worker> logger, IHttpClientFactory httpClientFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timeZone =
            TimeZoneInfo.FindSystemTimeZoneById("Europe/Amsterdam");

        var now = TimeZoneInfo.ConvertTime(
            DateTimeOffset.UtcNow,
            timeZone);

        var date = now.ToString("yyyy-MM-dd");
        
        var urlTemplate =
            Environment.GetEnvironmentVariable("EPG_URL_TEMPLATE")
            ?? throw new InvalidOperationException(
                "EPG_URL_TEMPLATE is not configured.");

        var url = urlTemplate.Replace("{date}", date);

        try
        {
            using var client = httpClientFactory.CreateClient();
            
            var response = await client.GetAsync(url, stoppingToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "EPG request for {Date} failed with HTTP {StatusCode}", date,
                    (int)response.StatusCode);

                return;
            }
            
            var content = await response.Content.ReadAsStringAsync(stoppingToken);

            try
            {
                using var document = JsonDocument.Parse(content);
                
                logger.LogInformation(
                    "EPG response for {Date} contains valid JSON", date);
            }
            catch (JsonException exception)
            {
                logger.LogError(
                    exception,
                    "EPG response for {Date} contains invalid JSON", date
                    );

                return;
            }
            
            logger.LogInformation(
                "Successfully fetched EPG data for {Date}",
                date);
            
            var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(content));
            var payloadHash = Convert.ToHexString(hashBytes);
            
            logger.LogInformation(
                "EPG response for {Date} has hash {Hash}",
                date,
                payloadHash);
            
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "EPG request for {Date} failed with exception",
                date);
        }
    }
}
