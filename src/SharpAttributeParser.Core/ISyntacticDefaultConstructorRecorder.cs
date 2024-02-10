namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

/// <summary>Records syntactic information about the unspecified arguments of optional constructor parameters.</summary>
public interface ISyntacticDefaultConstructorRecorder
{
    /// <summary>Attempts to record syntactic information about an unspecified argument of an optional constructor parameter.</summary>
    /// <param name="parameter">The constructor parameter.</param>
    /// <returns>A <see cref="bool"/> indicating whether syntactic information was successfully recorded.</returns>
    public abstract bool TryRecordArgument(IParameterSymbol parameter);
}
