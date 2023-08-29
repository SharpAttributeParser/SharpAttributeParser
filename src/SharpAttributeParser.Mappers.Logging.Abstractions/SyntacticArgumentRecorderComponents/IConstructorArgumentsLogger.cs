namespace SharpAttributeParser.Mappers.Logging.SyntacticArgumentRecorderComponents;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;

/// <summary>Handles logging for <see cref="ISyntacticRecorder"/> when related to constructor arguments.</summary>
public interface IConstructorArgumentsLogger
{
    /// <summary>Begins a log scope describing an attempt to record syntactic information about an argument of a constructor parameter.</summary>
    /// <param name="parameter">The constructor parameter.</param>
    /// <param name="syntax">The syntactic description of the constructor argument.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    public abstract IDisposable? BeginScopeRecordingNormalConstructorArgument(IParameterSymbol parameter, ExpressionSyntax syntax);

    /// <summary>Begins a log scope describing an attempt to record syntactic information about a <see langword="params"/>-argument of a constructor parameter.</summary>
    /// <param name="parameter">The constructor parameter.</param>
    /// <param name="elementSyntax">The syntactic description of each element in the <see langword="params"/>-argument.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    public abstract IDisposable? BeginScopeRecordingParamsConstructorArgument(IParameterSymbol parameter, IReadOnlyList<ExpressionSyntax> elementSyntax);

    /// <summary>Begins a log scope describing an attempt to record syntactic information about an unspecified argument of an optional constructor parameter.</summary>
    /// <param name="parameter">The constructor parameter.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    public abstract IDisposable? BeginScopeRecordingDefaultConstructorArgument(IParameterSymbol parameter);

    /// <summary>Logs a message describing a failed attempt to map a constructor parameter to a recorder.</summary>
    public abstract void FailedToMapConstructorParameterToRecorder();
}
