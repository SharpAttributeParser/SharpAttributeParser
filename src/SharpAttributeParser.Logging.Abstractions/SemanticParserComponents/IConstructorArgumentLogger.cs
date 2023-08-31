namespace SharpAttributeParser.Logging.SemanticParserComponents;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;

/// <summary>Handles logging for <see cref="ISemanticParser"/> when related to constructor arguments.</summary>
public interface IConstructorArgumentLogger
{
    /// <summary>Begins a log scope describing an attempt to parse the constructor arguments of an attribute.</summary>
    /// <param name="parameters">The constructor parameters.</param>
    /// <param name="arguments">The arguments of the constructor parameters.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    public abstract IDisposable? BeginScopeParsingConstructorArguments(IReadOnlyList<IParameterSymbol> parameters, IReadOnlyList<TypedConstant> arguments);

    /// <summary>Begins a log scope describing an attempt to parse an argument of a constructor parameter.</summary>
    /// <param name="parameter">The constructor parameter.</param>
    /// <param name="argument">The argument of the constructor parameter.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    public abstract IDisposable? BeginScopeParsingConstructorArgument(IParameterSymbol parameter, TypedConstant argument);

    /// <summary>Logs a message describing an unexpected number of constructor arguments.</summary>
    public abstract void UnexpectedNumberOfConstructorArguments();

    /// <summary>Logs a message describing a failed attempt to parse a constructor argument, as the argument was invalid.</summary>
    public abstract void InvalidConstructorArgument();
}
