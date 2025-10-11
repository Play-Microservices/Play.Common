using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Play.Common.Identity;

public static class Extensions
{
    public static WebApplicationBuilder AddJwtBearerAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.ConfigureOptions<ConfigureJwtBearerOptions>();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();
            
        return builder;
    }
}