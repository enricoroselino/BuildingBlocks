﻿using Nebx.API.BuildingBlocks.Services.TokenProvider;

namespace Nebx.API.BuildingBlocks.Configurations;

internal static class AuthenticationConfiguration
{
    public static IServiceCollection AddAuthenticationConfiguration(this IServiceCollection services)
    {
        services
            .AddOptions<TokenProviderOptions>()
            .BindConfiguration(TokenProviderOptions.Section)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<ITokenProvider, TokenProvider>();

        using var serviceProvider = services.BuildServiceProvider();
        var tokenProvider = serviceProvider.GetRequiredService<ITokenProvider>();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = tokenProvider.Scheme;
                options.DefaultChallengeScheme = tokenProvider.Scheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = tokenProvider.TokenValidationParameters;
            });

        return services;
    }
}