namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

/// <summary>Records the arguments of type parameters.</summary>
public interface ISemanticTypeRecorder
{
    /// <summary>Attempts to record an argument of a type parameter.</summary>
    /// <param name="parameter">The type parameter.</param>
    /// <param name="argument">The argument of the type parameter.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    public abstract bool TryRecordArgument(ITypeParameterSymbol parameter, ITypeSymbol argument);
}
