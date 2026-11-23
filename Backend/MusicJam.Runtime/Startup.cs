using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using MusicJam.Runtime.Clients.StudioClient;
using MusicJam.Runtime.Extensions;
using MusicJam.Runtime.Hubs;
using MusicJam.Runtime.Middlewares;
using MusicJam.Runtime.Services.GameService;
using MusicJam.Runtime.Services.InvitesService;
using MusicJam.Runtime.Services.UsersService;
using MusicJam.Runtime.Services.WaitingRoomService;
using MusicJam.Runtime.Storages.BindingsStorage;
using MusicJam.Runtime.Storages.GameStorage;
using MusicJam.Runtime.Telemetry;
using MusicJam.Runtime.Storages.InvitesStorage;
using MusicJam.Studio.Constants;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Shared;
using Shared.Auth;

namespace MusicJam.Runtime;

public class Startup(IConfiguration configuration)
{
    private IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .ConfigureSettings(configuration)
            .AddServices()
            .AddStorages()
            .AddTelemetry()
            .AddControllerOptions()
            .AddHubOptions();
        
        services.AddOptions<EnvironmentSettings>().BindConfiguration(EnvironmentSettings.SectionName);
        ConfigureAuthentication(services);
        ConfigureClient(services);
        services.AddAuthorization();

        services.AddHttpContextAccessor();
        services.AddTransient<ForwardAccessTokenHandler>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app
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
}
