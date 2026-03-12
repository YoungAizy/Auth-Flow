using Microsoft.Extensions.Caching.Memory;

namespace Auth.Infrastructure.Cache
{
    public class CacheMaintenanceWorker(
        IMemoryCache cache,
        ILogger<CacheMaintenanceWorker> logger,
        CacheHealthState state) : BackgroundService
    {
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(30);

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            logger.LogInformation("Cache Maintenance Worker started.");

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    if (state.IsMemoryPressureHigh && cache is MemoryCache concreteCache)
                    {
                        concreteCache.Compact(0.1);
                        logger.LogInformation("Cache compaction performed successfully.");
                        state.IsMemoryPressureHigh = false;
                    }

                    await Task.Delay(_checkInterval, ct);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred during cache maintenance.");
                }
            }
        }
    }
}