using System.Diagnostics;
using MusicJam.Studio.Requests.Auth;
using MusicJam.Studio.Telemetry;
using Shared;

namespace MusicJam.Studio.Services.AuthService;

internal class AuthServiceLoggingDecorator(
    IAuthService authService,
    ILogger<AuthServiceLoggingDecorator> logger) : IAuthService
{
    public async Task<Result<string, IAuthService.AuthError>> Login(LoginRequest request)
    {
        using var activity = StudioActivitySource.Source.StartActivity("AuthService.Login");
        
        logger.LogInformation(
            "Start login user with email = {Email}",
            request.Email);

        var loginResult = await authService.Login(request);
        if (loginResult.TryGetFault(out var error, out var token))
        {
            activity?.SetStatus(ActivityStatusCode.Error, error.ToString());
            logger.LogInformation(
                "Login user with email = {Email} finished with fault = {StatusCode}",
                request.Email,
                error);

            return error;
        }

        logger.LogInformation(
            "Login user with email = {Email} finished successfully",
            request.Email);

        return loginResult;
    }

    public async Task<Result<string, IAuthService.AuthError>> Register(RegisterRequest request)
    {
        using var activity = StudioActivitySource.Source.StartActivity("AuthService.Register");
        
        logger.LogInformation(
            "Start register user with email = {Email}",
            request.Email);

        var registerResult = await authService.Register(request);
        if (registerResult.TryGetFault(out var error, out var token))
        {
            activity?.SetStatus(ActivityStatusCode.Error, error.ToString());
            logger.LogInformation(
                "Register user with email = {Email} finished with fault = {StatusCode}",
                request.Email,
                error);

            return error;
        }

        logger.LogInformation(
            "Register user with email = {Email} finished successfully",
            request.Email);

        return registerResult;
    }
}
