namespace SharpAttributeParser.Mappers.Repositories;

using System;

/// <summary>Provides the default <see cref="IParameterComparer"/>.</summary>
internal static class DefaultParameterComparer
{
    /// <summary>The default <see cref="IParameterComparer"/>.</summary>
    public static IParameterComparer Comparer { get; } = new ParameterComparer(new TypeParameterComparer(StringComparer.OrdinalIgnoreCase), new ConstructorParameterComparer(StringComparer.OrdinalIgnoreCase), new NamedParameterComparer(StringComparer.OrdinalIgnoreCase));
}
