using System.Reflection;
using GreenPipes;
using GreenPipes.Configurators;
using MassTransit;
using MassTransit.Definition;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Common.Settings;

namespace Play.Common.MassTransit;

public static class Extensions
{
    private const string RabbitMq = "RABBITMQ";
    private const string ServiceBus = "SERVICEBUS";
    
    public static IServiceCollection AddMassTransitWithMessageBroker(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IRetryConfigurator>? configureRetries = null)
    {
        var serviceSetting = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>()!;
        switch (serviceSetting.MessageBroker.ToUpper())
        {
            case ServiceBus:
                services.AddMassTransitWithServiceBus(configureRetries);
                break;
            case RabbitMq:
            default:
                services.AddMassTransitWithRabbitMQ(configureRetries);
                break;
        }

        return services;
    }
    
    public static IServiceCollection AddMassTransitWithRabbitMQ(
        this IServiceCollection services,
        Action<IRetryConfigurator>? configureRetries = null)
    {
        services.AddMassTransit(configure =>
        {
            configure.AddConsumers(Assembly.GetEntryAssembly());
            configure.UsingPlayEconomyRabbitMQ(configureRetries);
        });

        return services;
    }
    
    public static IServiceCollection AddMassTransitWithServiceBus(
        this IServiceCollection services,
        Action<IRetryConfigurator>? configureRetries = null)
    {
        services.AddMassTransit(configure =>
        {
            configure.AddConsumers(Assembly.GetEntryAssembly());
            configure.UsingPlayEconomyServiceBus(configureRetries);
        });
        services.AddMassTransitHostedService();

        return services;
    }

    public static void UsingPlayEconomyMessageBroker(
        this IBusRegistrationConfigurator configure,
        IConfiguration configuration,
        Action<IRetryConfigurator>? configureRetries = null)
    {
        var serviceSetting = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>()!;
        switch (serviceSetting.MessageBroker.ToUpper())
        {
            case ServiceBus:
                configure.UsingPlayEconomyServiceBus(configureRetries);
                break;
            case RabbitMq:
            default:
                configure.UsingPlayEconomyRabbitMQ(configureRetries);
                break;
        }
    }

    public static void UsingPlayEconomyRabbitMQ(
        this IBusRegistrationConfigurator configure,
        Action<IRetryConfigurator>? configureRetries = null)
    {
        configure.UsingRabbitMq((context, configurator) => 
        {
            var configuration = context.GetService<IConfiguration>();
            var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
            var rabbitMQSettings = configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
            configurator.Host(rabbitMQSettings!.Host);
            configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings!.ServiceName, false));
            configureRetries ??= retryConfigurator => { retryConfigurator.Interval(3, TimeSpan.FromSeconds(5)); };
            configurator.UseMessageRetry(configureRetries);
        });
    }

    public static void UsingPlayEconomyServiceBus(
        this IBusRegistrationConfigurator configure,
        Action<IRetryConfigurator>? configureRetries = null)
    {
        configure.UsingAzureServiceBus((context, configurator) => 
        {
            var configuration = context.GetService<IConfiguration>();
            var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
            var serviceBusSettings = configuration.GetSection(nameof(ServiceBusSettings)).Get<ServiceBusSettings>();
            configurator.Host(serviceBusSettings!.ConnectionString);
            configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings!.ServiceName, false));
            configureRetries ??= retryConfigurator => { retryConfigurator.Interval(3, TimeSpan.FromSeconds(5)); };
            configurator.UseMessageRetry(configureRetries);
        });
    }
}