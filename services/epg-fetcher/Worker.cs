namespace EpgFetcher;

public class Worker(ILogger<Worker> logger, IHttpClientFactory httpClientFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var client = httpClientFactory.CreateClient();
        
        var date = DateTime.Now.ToString("yyyy-MM-dd");
        var url = $"https://www.ziggosport.nl/cache/site/ZiggosportNL/json/epg/epg-{date}.json";
        
        var response = await client.GetAsync(url, stoppingToken);
        
        var content = await response.Content.ReadAsStringAsync(stoppingToken);
        
        logger.LogInformation("Fetched EPG data: {Content}", content);
    }
}
