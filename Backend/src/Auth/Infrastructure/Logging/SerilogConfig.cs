using Serilog;

namespace Auth.Infrastructure.Logging
{
    public static class SerilogConfig
    {
        public static void ConfigureSerilog(this WebApplicationBuilder builder)
        {
            // 1. Initial "Bootstrap" logger for startup errors
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            // 2. Full configuration from appsettings.json
            builder.Host.UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                // Add OpenTelemetry sink if you're using it
                .WriteTo.OpenTelemetry(options => {
                    options.Endpoint = context.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] ?? "http://localhost:4317";
                    options.ResourceAttributes["service.name"] = "YourAppName";
                }));
        }
    }
}
