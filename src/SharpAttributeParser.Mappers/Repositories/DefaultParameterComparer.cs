namespace SharpAttributeParser.Mappers.Repositories;

using System;

internal static class DefaultParameterComparer
{
    public static IParameterComparer Comparer { get; } = new ParameterComparer(new TypeParameterComparer(StringComparer.OrdinalIgnoreCase), new ConstructorParameterComparer(StringComparer.OrdinalIgnoreCase), new NamedParameterComparer(StringComparer.OrdinalIgnoreCase));
}
