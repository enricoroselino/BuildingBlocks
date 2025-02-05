﻿using Microsoft.OpenApi.Models;

namespace Nebx.API.BuildingBlocks.Configurations;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerGenJwtAuthentication(this IServiceCollection services)
    {
        var securityScheme = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            In = ParameterLocation.Header,
            BearerFormat = "JWT",
            Scheme = "bearer",
            Reference = new OpenApiReference
            {
                Id = "Bearer",
                Type = ReferenceType.SecurityScheme
            },
            Description = "Please enter your JWT with this format: ''YOUR_TOKEN''",
        };

        var securityRequirement = new OpenApiSecurityRequirement { { securityScheme, Array.Empty<string>() } };

        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", securityScheme);
            options.AddSecurityRequirement(securityRequirement);
        });

        return services;
    }
}