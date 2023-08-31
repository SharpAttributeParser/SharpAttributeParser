namespace SharpAttributeParser.Mappers.Logging.SemanticArgumentRecorderComponents;

using Microsoft.CodeAnalysis;

using System;

/// <summary>Handles logging for <see cref="ISemanticRecorder"/> when related to constructor arguments.</summary>
public interface IConstructorArgumentLogger
{
    /// <summary>Begins a log scope describing an attempt to record an argument of a constructor parameter.</summary>
    /// <param name="parameter">The constructor parameter.</param>
    /// <param name="argument">The argument of the parameter.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    public abstract IDisposable? BeginScopeRecordingConstructorArgument(IParameterSymbol parameter, object? argument);

    /// <summary>Logs a message describing a failed attempt to map a constructor parameter to a recorder.</summary>
    public abstract void FailedToMapConstructorParameterToRecorder();
}
