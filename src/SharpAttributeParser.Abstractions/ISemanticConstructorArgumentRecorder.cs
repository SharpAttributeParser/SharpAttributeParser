namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System;

/// <summary>Records the arguments of constructor parameters.</summary>
public interface ISemanticConstructorArgumentRecorder
{
    /// <summary>Attempts to record an argument of a constructor parameter.</summary>
    /// <param name="parameter">The constructor parameter.</param>
    /// <param name="argument">The argument of the constructor parameter.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract bool TryRecordArgument(IParameterSymbol parameter, object? argument);
}
