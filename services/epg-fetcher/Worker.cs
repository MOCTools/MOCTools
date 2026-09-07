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
        
        var client = httpClientFactory.CreateClient();
        
        var response = await client.GetAsync(url, stoppingToken);
        
        var content = await response.Content.ReadAsStringAsync(stoppingToken);
        
        logger.LogInformation("Fetched EPG data: {Content}", content);
    }
}
