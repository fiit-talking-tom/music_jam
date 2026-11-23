using System.Buffers;
using System.Text.Json.Serialization;
using Amazon.S3;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MusicJam.Studio.Configurations;
using MusicJam.Studio.Infrastructure.Data;
using MusicJam.Studio.Services.AuthService;
using MusicJam.Studio.Services.PacksService;
using MusicJam.Studio.Services.QuestionsService;
using MusicJam.Studio.Services.TokenService;
using MusicJam.Studio.Services.TracksService;
using MusicJam.Studio.Storages;
using MusicJam.Studio.Storages.QuestionStorage;
using MusicJam.Studio.Storages.RoundStorage;
using MusicJam.Studio.Storages.TrackStorage;
using MusicJam.Studio.Storages.UsersStorage;
using MusicJam.Studio.Telemetry;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Shared.Auth;

namespace MusicJam.Studio.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services
            .AddScoped<IAuthService, AuthService>()
            .Decorate<IAuthService, AuthServiceLoggingDecorator>()
            .AddScoped<ITokenService, JwtTokenService>()
            .AddScoped<IQuestionsService, QuestionsService>()
            .Decorate<IQuestionsService, QuestionServiceLoggingDecorator>()
            .AddScoped<IPacksService, PacksService>()
            .Decorate<IPacksService, PacksServiceLoggingDecorator>()
            .AddScoped<ITracksService, TracksService>()
            .Decorate<ITracksService, TracksServiceLoggingDecorator>();
        return services;
    }

    public static IServiceCollection AddStorages(this IServiceCollection services)
    {
        services
            .AddScoped<IUserStorage, UserStorage>()
            .AddScoped<ITrackStorage, TrackStorage>()
            .AddScoped<IQuestionStorage, QuestionStorage>()
            .AddScoped<IRoundStorage, RoundStorage>()
            .AddScoped<IPackStorage, PackStorage>();
        return services;
    }

    public static IServiceCollection AddObjectStorage(this IServiceCollection services)
    {
        services.AddSingleton<IAmazonS3>(provider =>
        {
            var settings = provider
                .GetRequiredService<IOptions<ObjectStorageSettings>>()
                .Value;

            var config = new AmazonS3Config
            {
                ServiceURL = settings.ObjectStorageBaseUrl,
                ForcePathStyle = true
            };

            var client = new AmazonS3Client(settings.AccessKey, settings.SecretKey, config);
            return client;
        });

        return services;
    }

    public static IServiceCollection ConfigureSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .Configure<ObjectStorageSettings>(configuration.GetSection(nameof(ObjectStorageSettings)))
            .Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));

        return services;
    }
    
    public static IServiceCollection AddTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService("MusicJam.Studio"))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddSource(StudioActivitySource.Name)
                    .AddSource("Npgsql")
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(configuration["Jaeger:Endpoint"]);
                        options.Protocol = OtlpExportProtocol.HttpProtobuf;
                    });
            })
            .WithMetrics(metrics => metrics
                .AddPrometheusExporter()
                .AddHttpClientInstrumentation()
                .AddMeter("Npgsql")
                .AddAspNetCoreInstrumentation());
        return services;
    }
    
    public static IServiceCollection AddControllerOptions(this IServiceCollection services)
    {
        services.AddControllers(options =>
            {
                options.OutputFormatters.Insert(0, new
                    NewtonsoftJsonOutputFormatter(new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                        Converters = { new StringEnumConverter() }
                    }, ArrayPool<char>.Shared, options));
                options.RespectBrowserAcceptHeader = true;
            })
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        return services;
    }
}
