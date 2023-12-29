namespace SharpAttributeParser.Mappers;

using SharpAttributeParser.Patterns;

internal static class ArgumentPatternFactorySingleton
{
    public static IArgumentPatternFactory Singleton { get; } = new ArgumentPatternFactory();
}
