using System.Text;
using Elastic.Channels;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Elastic.Transport;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MusicJam.Runtime.Clients.StudioClient;
using MusicJam.Runtime.Extensions;
using MusicJam.Runtime.Hubs;
using MusicJam.Runtime.Middlewares;
using MusicJam.Studio.Constants;
using Serilog;
using Shared;
using Shared.Auth;

namespace MusicJam.Runtime;

public class Startup(
    IConfiguration configuration,
    IWebHostEnvironment env)
{
    private IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .ConfigureSettings(configuration)
            .AddServices()
            .AddStorages()
            .AddTelemetry(configuration)
            .AddControllerOptions()
            .AddHubOptions();
        
        services.AddOptions<EnvironmentSettings>().BindConfiguration(EnvironmentSettings.SectionName);
        ConfigureAuthentication(services);
        ConfigureClient(services);
        ConfigureSerilog(services);
        services.AddAuthorization();

        services.AddHttpContextAccessor();
        services.AddTransient<ForwardAccessTokenHandler>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app
            .UseSerilogRequestLogging()
            .UseMiddleware<SessionMiddleware>()
            .UseMiddleware<EnrichBindingContextMiddleware>()
            .UseRouting()
            .UseAuthentication()
            .UseAuthorization()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<GameHub>(GameHub.Path);
            })
            .UseOpenTelemetryPrometheusScrapingEndpoint();
    }

    private void ConfigureClient(IServiceCollection services)
    {
        services.AddHttpClient<IStudioClient, StudioClient>(client =>
            {
                var studioUrl = Configuration["Services:Studio"]
                                ?? throw new InvalidOperationException("Не задан адрес Studio: Services:Studio");
                client.BaseAddress = new Uri(studioUrl);
            })
            .AddHttpMessageHandler<ForwardAccessTokenHandler>();
    }
    
    private void ConfigureAuthentication(IServiceCollection services)
    {
        var jwtSettings = new JwtSettings();
        Configuration.GetSection(nameof(JwtSettings)).Bind(jwtSettings);
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = AuthConstants.DefaultAuthorizationScheme;
            })
            .AddJwtBearer(AuthConstants.DefaultAuthorizationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    ValidateIssuerSigningKey = true,
                };
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
        var elasticUrl = configuration["Elastic:Url"];
        var elasticUsername = configuration["Elastic:Username"] ?? "elastic";
        var elasticPassword = configuration["Elastic:Password"] ?? string.Empty;

        services.AddSerilog((_, lc) =>
        {
            lc.Enrich.FromLogContext()
                .Enrich.WithProperty("Environment", env.EnvironmentName)
                .Enrich.WithProperty("Service", "MusicJam.Runtime")
                .ReadFrom.Configuration(configuration);

            if (!string.IsNullOrEmpty(elasticUrl))
                lc.WriteTo.Elasticsearch([new Uri(elasticUrl)], opts =>
                    {
                        opts.DataStream =
                            new DataStreamName("logs", "musicjam", env.EnvironmentName.ToLowerInvariant());
                        opts.BootstrapMethod = BootstrapMethod.Failure;
                        opts.ConfigureChannel = channelOpts =>
                        {
                            channelOpts.BufferOptions = new BufferOptions
                            {
                                ExportMaxConcurrency = 10
                            };
                        };
                    },
                    transport =>
                    {
                        transport.Authentication(new BasicAuthentication(elasticUsername, elasticPassword));
                    });
        });
    }
}
