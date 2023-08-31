namespace SharpAttributeParser;

using Microsoft.Extensions.DependencyInjection;

using SharpAttributeParser.Logging;

using System;

/// <summary>Allows the services of <i>SharpAttributeParser</i> to be registered with a <see cref="IServiceCollection"/>.</summary>
public static class SharpAttributeParserServices
{
    /// <summary>Registers the services of <i>SharpAttributeParser</i> with the provided <see cref="IServiceCollection"/>.</summary>
    /// <param name="services">The <see cref="IServiceCollection"/> with which services are registered.</param>
    /// <returns>The provided <see cref="IServiceCollection"/>, so that calls can be chained.</returns>
    public static IServiceCollection AddSharpAttributeParser(this IServiceCollection services)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddSharpAttributeParserLogging();

        services.AddSingleton<ICombinedParser, CombinedParser>();
        services.AddSingleton<ISemanticParser, SemanticParser>();
        services.AddSingleton<ISyntacticParser, SyntacticParser>();

        return services;
    }
}
