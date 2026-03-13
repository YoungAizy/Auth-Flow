using Auth.Data;
using Auth.Infrastructure.Aws;
using Auth.Infrastructure.Cache;
using Auth.Infrastructure.ExceptionHandlers;
using Auth.Infrastructure.HealthChecks;
using Auth.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Amazon.CognitoIdentityProvider;

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

            services.AddSingleton<UserMapper>();

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
            services.AddMemoryCache();
            services.AddHostedService<CacheMaintenanceWorker>();

            // Add other infrastructure services as needed (e.g., logging, AWS services, etc.)
            //services.AddAWSService(); // Add AWS services to the container
            services.AddSingleton<AmazonCognitoIdentityProviderClient>(sp=>
                sp.GetRequiredService<CognitoClientFactory>().CreateClient()
            );

            return services;
        }
    }
}
