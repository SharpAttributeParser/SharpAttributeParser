﻿namespace SharpAttributeParser.Mappers.Logging.CombinedArgumentRecorderComponents;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;

/// <summary>Handles logging for <see cref="ICombinedRecorder"/> when related to type arguments.</summary>
public interface ITypeArgumentLogger
{
    /// <summary>Begins a log scope describing an attempt to record an argument of a type parameter.</summary>
    /// <param name="parameter">The type parameter.</param>
    /// <param name="argument">The argument of the parameter.</param>
    /// <param name="syntax">The syntactic description of the argument.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    public abstract IDisposable? BeginScopeRecordingTypeArgument(ITypeParameterSymbol parameter, ITypeSymbol argument, ExpressionSyntax syntax);

    /// <summary>Logs a message describing a failed attempt to map a type parameter to a recorder.</summary>
    public abstract void FailedToMapTypeParameterToRecorder();
}
