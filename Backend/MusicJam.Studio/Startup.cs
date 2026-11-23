using System.Buffers;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MusicJam.Studio.Constants;
using MusicJam.Studio.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Text.Json.Serialization;
using MusicJam.Studio.Infrastructure.Data;
using MusicJam.Studio.Telemetry;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Shared.Auth;

namespace MusicJam.Studio;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .ConfigureSettings(configuration)
            .AddObjectStorage()
            .AddServices()
            .AddStorages()
            .AddTelemetry()
            .AddControllerOptions();

        
        services.AddAuthorization();
        ConfigureAuthentication(services);
        ConfigureDbContext(services);
    }

    private void ConfigureDbContext(IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .UseSnakeCaseNamingConvention());
    }
    
    private void ConfigureAuthentication(IServiceCollection services)
    {
        var jwtSettings = new JwtSettings();
        configuration.GetSection(nameof(JwtSettings)).Bind(jwtSettings);

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = AuthConstants.DefaultAuthorizationScheme;
            })
            .AddJwtBearer(
                AuthConstants.DefaultAuthorizationScheme,
                options =>
                {
                    options.TokenValidationParameters = GetTokenValidationParameters(jwtSettings);
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            context.Token = context.Request.Cookies[AuthConstants.AccessToken];
                            return Task.CompletedTask;
                        }
                    };
                });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app
            .UseRouting()
            .UseAuthentication()
            .UseAuthorization()
            .UseEndpoints(endpoints => { endpoints.MapControllers(); })
            .UseOpenTelemetryPrometheusScrapingEndpoint();
    }

    private static TokenValidationParameters GetTokenValidationParameters(JwtSettings jwtSettings) => new()
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
        ValidateIssuerSigningKey = true,
    };
}
