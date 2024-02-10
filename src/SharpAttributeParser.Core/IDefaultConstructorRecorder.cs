namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

/// <summary>Records the unspecified arguments of optional constructor parameters, together with syntactic information about the arguments.</summary>
public interface IDefaultConstructorRecorder
{
    /// <summary>Attempts to record an unspecified argument of an optional constructor parameter, together with syntactic information about the argument.</summary>
    /// <param name="parameter">The constructor parameter.</param>
    /// <param name="argument">The argument of the constructor parameter.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    public abstract bool TryRecordArgument(IParameterSymbol parameter, object? argument);
}
