﻿using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Nebx.API.BuildingBlocks.Application.Behaviors;

namespace Nebx.API.BuildingBlocks.Contract.Extensions;

public static class MediatorExtensions
{
    public static IServiceCollection AddMediatorFromAssemblies(this IServiceCollection services,
        params Assembly[] assemblies)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(assemblies);
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        // validation added as part of mediator
        services.AddValidatorsFromAssemblies(assemblies);
        return services;
    }
}