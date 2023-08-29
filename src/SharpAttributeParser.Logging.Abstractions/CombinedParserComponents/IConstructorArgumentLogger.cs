namespace SharpAttributeParser.Logging.CombinedParserComponents;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;

/// <summary>Handles logging for <see cref="ICombinedParser"/> when related to constructor arguments.</summary>
public interface IConstructorArgumentLogger
{
    /// <summary>Begins a log scope describing an attempt to propagate the parsed constructor arguments of an attribute.</summary>
    /// <param name="semanticArguments">The semantically parsed arguments.</param>
    /// <param name="nonParamsSyntacticArguments">The syntactically parsed non-<see langword="params"/>-arguments.</param>
    /// <param name="paramsSyntacticArguments">The syntactically parsed <see langword="params"/>-arguments.</param>
    /// <param name="defaultSyntacticArguments">The syntactically parsed arguments, which were not explicitly specified.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    public abstract IDisposable? BeginScopePropagateParsedConstructorArguments(IReadOnlyDictionary<IParameterSymbol, object?> semanticArguments, IReadOnlyDictionary<IParameterSymbol, ExpressionSyntax> nonParamsSyntacticArguments, IReadOnlyDictionary<IParameterSymbol, IReadOnlyList<ExpressionSyntax>> paramsSyntacticArguments, ISet<IParameterSymbol> defaultSyntacticArguments);

    /// <summary>Begins a log scope describing an attempt to propagate a semantically parsed argument of a constructor parameter.</summary>
    /// <param name="parameter">The constructor parameter.</param>
    /// <param name="argument">The argument of the constructor parameter.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    public abstract IDisposable? BeginScopePropagateSemanticallyParsedConstructorArgument(IParameterSymbol parameter, object? argument);

    /// <summary>Begins a log scope describing an attempt to propagate a syntactically parsed argument of a constructor parameter.</summary>
    /// <param name="syntax">The syntactic description of the argument.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    public abstract IDisposable? BeginScopePropagateSyntacticallyParsedNormalConstructorArgument(ExpressionSyntax syntax);

    /// <summary>Begins a log scope describing an attempt to propagate a syntactically parsed <see langword="params"/>-argument of a constructor parameter..</summary>
    /// <param name="elementSyntax">The syntactic description of each element in the <see langword="params"/>-argument.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    public abstract IDisposable? BeginScopePropagateSyntacticallyParsedParamsConstructorArgument(IReadOnlyList<ExpressionSyntax> elementSyntax);

    /// <summary>Begins a log scope describing an attempt to propagate a syntactically parsed unspecified argument of an optional constructor parameter.</summary>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    public abstract IDisposable? BeginScopePropagateSyntacticallyParsedDefaultConstructorArgument();

    /// <summary>Logs a message describing different number of semantically and syntactically parsed constructor arguments.</summary>
    public abstract void DifferentNumberOfSemanticallyAndSyntacticallyParsedConstructorArguments();

    /// <summary>Logs a message describing a constructor argument having been parsed semantically, but not syntactically.</summary>
    public abstract void ConstructorArgumentOnlyRecordedSemantically();
}
