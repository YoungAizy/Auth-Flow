using Auth.Infrastructure.Cache;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Auth.Infrastructure.HealthChecks
{
    public class MemoryCacheHealthCheck(CacheHealthState state) : IHealthCheck
    {
        private const long MaxMemoryAllowed = 500 * 1024 * 1024; // 500 MB

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
        {
            // Get the current estimated size of the cache
            // Note: MemoryCache doesn't provide exact size easily, so we check process memory or specific metrics
            var currentMemory = GC.GetTotalMemory(false);

            if (currentMemory > MaxMemoryAllowed)
            {
                state.IsMemoryPressureHigh = true;
                return Task.FromResult(HealthCheckResult.Degraded(
                    $"Memory usage is high: {currentMemory / 1024 / 1024}MB"));
            }

            state.IsMemoryPressureHigh = false;
            return Task.FromResult(HealthCheckResult.Healthy("Cache memory is within limits."));
        }
    }
}
