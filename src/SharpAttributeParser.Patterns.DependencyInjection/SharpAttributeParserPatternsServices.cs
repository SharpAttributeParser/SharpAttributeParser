namespace SharpAttributeParser.Patterns;

using Microsoft.Extensions.DependencyInjection;

using System;

/// <summary>Allows the services of <i>SharpAttributeParser.Patterns</i> to be registered with a <see cref="IServiceCollection"/>.</summary>
public static class SharpAttributeParserPatternsServices
{
    /// <summary>Registers the services of <i>SharpAttributeParser.Patterns</i> with the provided <see cref="IServiceCollection"/>.</summary>
    /// <param name="services">The <see cref="IServiceCollection"/> with which services are registered.</param>
    /// <returns>The provided <see cref="IServiceCollection"/>, so that calls can be chained.</returns>
    /// <exception cref="ArgumentNullException"/>
    public static IServiceCollection AddSharpAttributeParserPatterns(this IServiceCollection services)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddSingleton<IArgumentPatternFactory, ArgumentPatternFactory>();

        return services;
    }
}
