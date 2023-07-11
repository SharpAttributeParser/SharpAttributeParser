namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;

/// <summary>Records syntactical information about the arguments of an attribute.</summary>
public interface ISyntacticAttributeRecorder
{
    /// <summary>Attempts to record syntactical information about the argument of a type-parameter.</summary>
    /// <param name="parameter">The <see cref="ITypeParameterSymbol"/> representing the type-parameter.</param>
    /// <param name="syntax">The <see cref="ExpressionSyntax"/>, syntactically describing the argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether the syntactical information was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract bool TryRecordTypeArgumentSyntax(ITypeParameterSymbol parameter, ExpressionSyntax syntax);

    /// <summary>Attempts to record syntactical information about the argument of a constructor parameter.</summary>
    /// <param name="parameter">The <see cref="IParameterSymbol"/> representing the parameter.</param>
    /// <param name="syntax">The <see cref="ExpressionSyntax"/>, syntactically describing the argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether the syntactical information was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract bool TryRecordConstructorArgumentSyntax(IParameterSymbol parameter, ExpressionSyntax syntax);

    /// <summary>Attempts to record syntactical information about the <see langword="params"/>-argument of a constructor parameter.</summary>
    /// <param name="parameter">The <see cref="IParameterSymbol"/> representing the parameter.</param>
    /// <param name="elementSyntax">The <see cref="ExpressionSyntax"/>, syntactically describing each element in the <see langword="params"/>-argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether the syntactical information was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract bool TryRecordConstructorParamsArgumentSyntax(IParameterSymbol parameter, IReadOnlyList<ExpressionSyntax> elementSyntax);

    /// <summary>Attempts to record syntactical information about the argument of a named parameter.</summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="syntax">The <see cref="ExpressionSyntax"/>, syntactically describing the argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether the syntactical information was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract bool TryRecordNamedArgumentSyntax(string parameterName, ExpressionSyntax syntax);
}

/// <summary>Records syntactical information about the arguments of an attribute.</summary>
/// <typeparam name="TRecord">The type to which the <see cref="ISemanticAttributeRecorder"/> records syntactical information.</typeparam>
public interface ISyntacticAttributeRecorder<out TRecord> : ISyntacticAttributeRecorder
{
    /// <summary>Retrieves the <typeparamref name="TRecord"/>, representing the recorded syntactical information.</summary>
    /// <returns>The <typeparamref name="TRecord"/> representing the recorded syntactical information.</returns>
    /// <exception cref="InvalidOperationException"/>
    public abstract TRecord GetRecord();
}
