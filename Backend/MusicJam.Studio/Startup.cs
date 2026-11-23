using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MusicJam.Studio.Constants;
using MusicJam.Studio.Extensions;
using Elastic.Channels;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Elastic.Transport;
using MusicJam.Studio.Infrastructure.Data;
using Serilog;
using Shared.Auth;

namespace MusicJam.Studio;

public class Startup(
    IConfiguration configuration,
    IWebHostEnvironment env)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .ConfigureSettings(configuration)
            .AddObjectStorage()
            .AddServices()
            .AddStorages()
            .AddTelemetry(configuration)
            .AddControllerOptions();
        
        services.AddAuthorization();
        ConfigureAuthentication(services);
        ConfigureDbContext(services);
        ConfigureSerilog(services);
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
    
    private void ConfigureSerilog(IServiceCollection services)
    {
        var elasticUrl = configuration["Elastic:Url"]
                         ?? throw new InvalidOperationException("Не задан адрес Elasticsearch: Elastic:Url");
        var elasticUsername = configuration["Elastic:Username"] ?? "elastic";
        var elasticPassword = configuration["Elastic:Password"] ?? string.Empty;

        services.AddSerilog((_, lc) => lc.Enrich.FromLogContext()
            .WriteTo.Elasticsearch([new Uri(elasticUrl)], opts =>
            {
                opts.DataStream = new DataStreamName("logs", "musicjam", env.EnvironmentName.ToLowerInvariant());
                opts.BootstrapMethod = BootstrapMethod.Failure;
                opts.ConfigureChannel = channelOpts =>
                {
                    channelOpts.BufferOptions = new BufferOptions
                    {
                        ExportMaxConcurrency = 10
                    };
                };
            }, transport =>
            {
                transport.Authentication(new BasicAuthentication(elasticUsername, elasticPassword));
            })
            .Enrich.WithProperty("Environment", env.EnvironmentName)
            .Enrich.WithProperty("Service", "MusicJam.Studio")
            .ReadFrom.Configuration(configuration));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app
            .UseSerilogRequestLogging()
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
