using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.SignalR;
using MusicJam.Runtime.Hubs;
using MusicJam.Runtime.Services.GameService;
using MusicJam.Runtime.Services.InvitesService;
using MusicJam.Runtime.Services.UsersService;
using MusicJam.Runtime.Services.WaitingRoomService;
using MusicJam.Runtime.Storages.BindingsStorage;
using MusicJam.Runtime.Storages.GameStorage;
using MusicJam.Runtime.Storages.InvitesStorage;
using MusicJam.Runtime.Telemetry;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Shared.Auth;

namespace MusicJam.Runtime.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IWaitingRoomService, WaitingRoomService>()
            .Decorate<IWaitingRoomService, WaitingRoomServiceLoggingDecorator>()
            .AddScoped<IGameService, GameService>()
            .Decorate<IGameService, GameServiceLoggingDecorator>()
            .AddScoped<IUsersService, UsersService>()
            .Decorate<IUsersService, UsersServiceLoggingDecorator>()
            .AddScoped<IInvitesService, InvitesService>()
            .Decorate<IInvitesService, InvitesServiceLoggerDecorator>();
        return services;
    }

    public static IServiceCollection AddStorages(this IServiceCollection services)
    {
        services
            .AddSingleton<IGameStorage, GameStorage>()
            .AddSingleton<IBindingsStorage, InMemoryBindingsStorage>()
            .AddSingleton<IInvitesStorage, InMemoryInvitesStorage>();
        return services;
    }

    public static IServiceCollection AddTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService("MusicJam.Runtime"))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddSource(RuntimeActivitySource.Name)
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(configuration["Jaeger:Endpoint"]);
                        options.Protocol = OtlpExportProtocol.HttpProtobuf;
                    });
            })
            .WithMetrics(metrics => metrics
                .AddPrometheusExporter()
                .AddProcessInstrumentation()
                .AddRuntimeInstrumentation()
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation());
        return services;
    }

    public static IServiceCollection AddControllerOptions(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)));
        return services;
    }
    
    public static IServiceCollection AddHubOptions(this IServiceCollection services)
    {
        services.AddSignalR()
            .AddJsonProtocol(o => o.PayloadSerializerOptions.Converters.Add(
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)));
        return services;
    }

    public static IServiceCollection ConfigureSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
        return services;
    }
}