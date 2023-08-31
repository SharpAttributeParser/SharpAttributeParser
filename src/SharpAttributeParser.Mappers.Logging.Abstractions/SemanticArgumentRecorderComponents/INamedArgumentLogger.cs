namespace SharpAttributeParser.Mappers.Logging.SemanticArgumentRecorderComponents;

using SharpAttributeParser.Mappers.MappedRecorders;

using System;

/// <summary>Handles logging for <see cref="ISemanticRecorder"/> when related to named arguments.</summary>
public interface INamedArgumentLogger
{
    /// <summary>Begins a log scope describing an attempt to record an argument of a named parameter.</summary>
    /// <param name="parameterName">The name of the named parameter.</param>
    /// <param name="argument">The argument of the parameter.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    public abstract IDisposable? BeginScopeRecordingNamedArgument(string parameterName, object? argument);

    /// <summary>Logs a message describing a failed attempt to map a named parameter to a <see cref="IMappedSemanticNamedArgumentRecorder"/>.</summary>
    public abstract void FailedToMapNamedParameterToRecorder();
}
