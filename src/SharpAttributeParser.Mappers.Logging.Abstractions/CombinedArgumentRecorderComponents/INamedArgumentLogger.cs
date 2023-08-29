namespace SharpAttributeParser.Mappers.Logging.CombinedArgumentRecorderComponents;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;

/// <summary>Handles logging for <see cref="ICombinedRecorder"/> when related to named arguments.</summary>
public interface INamedArgumentLogger
{
    /// <summary>Begins a log scope describing an attempt to record an argument of a named parameter.</summary>
    /// <param name="parameterName">The name of the named parameter.</param>
    /// <param name="argument">The named argument.</param>
    /// <param name="syntax">The syntactic description of the named argument.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    public abstract IDisposable? BeginScopeRecordingNamedArgument(string parameterName, object? argument, ExpressionSyntax syntax);

    /// <summary>Logs a message describing a failed attempt to map a named parameter to a recorder.</summary>
    public abstract void FailedToMapNamedParameterToRecorder();
}
