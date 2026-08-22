using Azure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Play.Common.Settings;

namespace Play.Common.Configuration;

public static class Extentions
{
    public static WebApplicationBuilder AddKeyVaultConfiguration(
        this WebApplicationBuilder builder,
        string keyVaultName)
    {
        if (builder.Environment.IsProduction())
        {
            builder.Configuration.AddAzureKeyVault(
                new Uri($"https://{keyVaultName}.vault.azure.net/"),
                new DefaultAzureCredential());
        }

        return builder;
    }
    
    public static IHostBuilder ConfigureAzureKeyVault(this IHostBuilder hostBuilder)
    {
        return hostBuilder.ConfigureAppConfiguration((context, builder) =>
        {
            if (context.HostingEnvironment.IsProduction())
            {
                var configuration = builder.Build();
                var serviceSettings = configuration.Get<ServiceSettings>();
                builder.AddAzureKeyVault(
                    new Uri($"https://{serviceSettings!.KeyVaultName}.vault.azure.net/"),
                    new DefaultAzureCredential());
            }
        });
    }
}