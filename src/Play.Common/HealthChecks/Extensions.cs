using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using Play.Common.Settings;

namespace Play.Common.HealthChecks;

public static class Extensions
{
    private const string Name = "mongodb";
    private const string ReadyTagName = "ready";
    private const string LiveTagName = "live";
    private const string HealthEndpointName = "health";
    private const int DefaultTimeoutInSeconds = 3;
    
    public static IHealthChecksBuilder AddMongoDb(
        this IHealthChecksBuilder builder,
        TimeSpan? timeout = null)
    {
        return builder.Add(new HealthCheckRegistration(
            Name,
            serviceProvider =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings))
                    .Get<MongoDbSettings>();
                var mongoClient = new MongoClient(mongoDbSettings!.ConnectionString);
                return new MongoDbHealthCheck(mongoClient);
            },
            HealthStatus.Unhealthy,
            [ReadyTagName],
            timeout ?? TimeSpan.FromSeconds(DefaultTimeoutInSeconds))
        );
    }

    public static void MapPlayEconomyHealthChecks(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHealthChecks($"/{HealthEndpointName}/{ReadyTagName}", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains(ReadyTagName)
        });
        endpoints.MapHealthChecks($"/{HealthEndpointName}/{LiveTagName}", new HealthCheckOptions
        {
            Predicate = _ => false
        });
    }
}