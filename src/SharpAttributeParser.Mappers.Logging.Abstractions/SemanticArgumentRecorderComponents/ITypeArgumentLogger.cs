namespace SharpAttributeParser.Mappers.Logging.SemanticArgumentRecorderComponents;

using Microsoft.CodeAnalysis;

using System;

/// <summary>Handles logging for <see cref="ISemanticRecorder"/> when related to type arguments.</summary>
public interface ITypeArgumentLogger
{
    /// <summary>Begins a log scope describing an attempt to record an argument of a type parameter.</summary>
    /// <param name="parameter">The type parameter.</param>
    /// <param name="argument">The argument of the parameter.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    public abstract IDisposable? BeginScopeRecordingTypeArgument(ITypeParameterSymbol parameter, ITypeSymbol argument);

    /// <summary>Logs a message describing a failed attempt to map a type parameter to a recorder.</summary>
    public abstract void FailedToMapTypeParameterToRecorder();
}
