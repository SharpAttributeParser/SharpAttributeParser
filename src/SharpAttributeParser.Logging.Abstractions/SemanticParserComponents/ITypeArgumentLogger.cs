namespace SharpAttributeParser.Logging.SemanticParserComponents;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;

/// <summary>Handles logging for <see cref="ISemanticParser"/> when related to type arguments.</summary>
public interface ITypeArgumentLogger
{
    /// <summary>Begins a log scope describing an attempt to parse the type arguments of an attribute.</summary>
    /// <param name="parameters">The type parameters.</param>
    /// <param name="arguments">The arguments of the type parameters.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    public abstract IDisposable? BeginScopeParsingTypeArguments(IReadOnlyList<ITypeParameterSymbol> parameters, IReadOnlyList<ITypeSymbol> arguments);

    /// <summary>Begins a log scope describing an attempt to parse the argument of a type parameter.</summary>
    /// <param name="parameter">The type parameter.</param>
    /// <param name="argument">The argument of the type parameter.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    public abstract IDisposable? BeginScopeParsingTypeArgument(ITypeParameterSymbol parameter, ITypeSymbol argument);

    /// <summary>Logs a message describing an unexpected number of type arguments.</summary>
    public abstract void UnexpectedNumberOfTypeArguments();

    /// <summary>Logs a message describing a failed attempt to parse a type argument, as the argument was invalid.</summary>
    public abstract void InvalidTypeArgument();
}
