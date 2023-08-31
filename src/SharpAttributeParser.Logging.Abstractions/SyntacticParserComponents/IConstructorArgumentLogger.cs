namespace SharpAttributeParser.Logging.SyntacticParserComponents;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;

/// <summary>Handles logging for <see cref="ISyntacticParser"/> when related to constructor arguments.</summary>
public interface IConstructorArgumentLogger
{
    /// <summary>Begins a log scope describing an attempt to parse the constructor arguments of an attribute.</summary>
    /// <param name="attributeData">The syntactic description of the attribute.</param>
    /// <param name="attributeSyntax">The syntactic description of the attribute.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    public abstract IDisposable? BeginScopeParsingConstructorArguments(AttributeData attributeData, AttributeSyntax attributeSyntax);

    /// <summary>Begins a log scope describing an attempt to parse an argument of a constructor parameter.</summary>
    /// <param name="parameter">The constructor parameter.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    public abstract IDisposable? BeginScopeParsingConstructorArgument(IParameterSymbol parameter);

    /// <summary>Begins a log scope describing an attempt to parse an argument of a constructor parameter.</summary>
    /// <param name="syntax">The syntactic description of the argument.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    public abstract IDisposable? BeginScopeParsingNormalConstructorArgument(ExpressionSyntax syntax);

    /// <summary>Begins a log scope describing an attempt to parse a <see langword="params"/>-argument of a constructor parameter.</summary>
    /// <param name="elementSyntax">The syntactic description of each element in the <see langword="params"/>-argument.</param>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    public abstract IDisposable? BeginScopeParsingParamsConstructorArgument(IReadOnlyList<ExpressionSyntax> elementSyntax);

    /// <summary>Begins a log scope describing an attempt to parse an unspecified argument of an optional constructor parameter.</summary>
    /// <returns>The <see cref="IDisposable"/> used to close the log scope.</returns>
    public abstract IDisposable? BeginScopeParsingDefaultConstructorArgument();

    /// <summary>Logs a message describing one or more required constructor argument that was not provided.</summary>
    public abstract void MissingOneOrMoreRequiredConstructorArgument();

    /// <summary>Logs a message describing an out-of-order labelled constructor argument being followed by an unlabelled constructor argument.</summary>
    public abstract void OutOfOrderLabelledConstructorArgumentsFollowedByUnlabelled();

    /// <summary>Logs a message describing an unexpected number of semantic constructor arguments.</summary>
    public abstract void UnexpectedNumberOfSemanticConstructorArguments();

    /// <summary>Logs a message describing a labelled constructor argument not having a matching parameter.</summary>
    public abstract void LabelledConstructorArgumentHadNoMatchingParameter();
}
