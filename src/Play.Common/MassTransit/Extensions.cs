using System.Reflection;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Common.Settings;

namespace Play.Common.MassTransit;

public static class Extensions
{
    public static IServiceCollection AddMassTransitWithRabbitMQ(
        this IServiceCollection services,
        Action<IRetryConfigurator>? configureRetries = null)
    {
        services.AddMassTransit(configure =>
        {
            configure.AddConsumers(Assembly.GetEntryAssembly());

            UsingPlayEconomyRabbitMQ(configure, configureRetries);
        });

        return services;
    }
    
    public static WebApplicationBuilder AddMassTransitWithRabbitMQ(
        this WebApplicationBuilder builder,
        Action<IRetryConfigurator>? configureRetries = null)
    {
        builder.Services.AddMassTransit(configure =>
        {
            configure.AddConsumers(Assembly.GetEntryAssembly());

            UsingPlayEconomyRabbitMQ(configure, configureRetries);
        });

        return builder;
    }

    public static void UsingPlayEconomyRabbitMQ(
        this IBusRegistrationConfigurator configure,
        Action<IRetryConfigurator>? configureRetries = null)
    {
        configure.UsingRabbitMq((context, configurator) => 
        {
            var configuration = context.GetService<IConfiguration>();
            var serviceSettings = configuration!.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
            var rabbitMQSettings = configuration!.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
            configurator.Host(rabbitMQSettings!.Host);
            configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings!.ServiceName, false));
            configureRetries ??= retryConfigurator => { retryConfigurator.Interval(3, TimeSpan.FromSeconds(5)); };
            configurator.UseMessageRetry(configureRetries);
        });
    }
}