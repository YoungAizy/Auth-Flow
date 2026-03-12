using Auth.Data;
using Auth.Infrastructure.Cache;
using Auth.Infrastructure.ExceptionHandlers;
using Auth.Infrastructure.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Auth.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // CONFIGURE POSTGRESQL DATABASE
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"])
                .AddNpgSql(connectionString!, tags: ["ready", "db"])
                .AddCheck<MemoryCacheHealthCheck>(
                    "Memory_Cache_Check",
                    failureStatus: HealthStatus.Degraded,
                    tags: [ "cache", "memory" ]);

            services.AddDbContext<AppDBContext>(options =>
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    // Resiliency for abrupt connection drops
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorCodesToAdd: null);
                }));

            // CONFIGURE OPENTELEMETRY
            var serviceName = configuration["OTEL_SERVICE_NAME"] ?? "YourAppName";

            services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService(serviceName))
                .WithTracing(tracing => tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddNpgsql() // Traces PostgreSQL queries
                    .AddOtlpExporter())
                .WithMetrics(metrics => metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddOtlpExporter());

            // EXCEPTION HANDLING
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();
            
            // CACHING
            services.AddHostedService<CacheMaintenanceWorker>();

            return services;
        }
    }
}
