namespace EpgFetcher;

public class Worker(ILogger<Worker> logger, IHttpClientFactory httpClientFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var client = httpClientFactory.CreateClient();
        
        var response = await client.GetAsync("https://www.ziggosport.nl/cache/site/ZiggosportNL/json/epg/epg-2026-09-06.json", stoppingToken);
        
        var content = await response.Content.ReadAsStringAsync(stoppingToken);
        
        logger.LogInformation("Fetched EPG data: {Content}", content);
    }
}
