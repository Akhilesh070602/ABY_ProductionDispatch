using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PackingDisplay.Services;

namespace PackingDisplay.Workers
{
    public class SapWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SapWorker> _logger;

        public SapWorker(IServiceScopeFactory scopeFactory, ILogger<SapWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SAP Worker Started");

            // ✅ Optional: delay at startup (avoid immediate SAP load)
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<FetchDataService>();

                    _logger.LogInformation("SAP Fetch Started at: {time}", DateTime.Now);

                    await service.FetchAndStoreData();

                    _logger.LogInformation("SAP Fetch Completed at: {time}", DateTime.Now);
                }
                catch (Exception ex)
                {
                    // ❌ Don't crash app — log error instead
                    _logger.LogError(ex, "Error while fetching data from SAP");
                }

                // ✅ Wait before next run
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }

            _logger.LogInformation("SAP Worker Stopped");
        }
    }
}