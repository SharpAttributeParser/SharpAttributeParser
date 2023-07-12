namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;

/// <summary>Records the arguments of an attribute, together with syntactical information about the arguments.</summary>
public interface IAttributeRecorder
{
    /// <summary>Attempts to record the argument of a type-parameter.</summary>
    /// <param name="parameter">The <see cref="ITypeParameterSymbol"/> representing the type-parameter.</param>
    /// <param name="argument">The argument of the type-parameter.</param>
    /// <param name="syntax">The <see cref="ExpressionSyntax"/>, syntactically describing the argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract bool TryRecordTypeArgument(ITypeParameterSymbol parameter, ITypeSymbol argument, ExpressionSyntax syntax);

    /// <summary>Attempts to record the argument of a constructor parameter.</summary>
    /// <param name="parameter">The <see cref="IParameterSymbol"/> representing the parameter.</param>
    /// <param name="argument">The argument of the parameter.</param>
    /// <param name="syntax">The <see cref="ExpressionSyntax"/>, syntactically describing the argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract bool TryRecordConstructorArgument(IParameterSymbol parameter, object? argument, ExpressionSyntax syntax);

    /// <summary>Attempts to record the <see langword="params"/>-argument of a constructor parameter.</summary>
    /// <param name="parameter">The <see cref="IParameterSymbol"/> representing the parameter.</param>
    /// <param name="argument">Theargument of the parameter.</param>
    /// <param name="elementSyntax">The <see cref="ExpressionSyntax"/>, syntactically describing each element of the <see langword="params"/>-argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract bool TryRecordConstructorParamsArgument(IParameterSymbol parameter, object? argument, IReadOnlyList<ExpressionSyntax> elementSyntax);

    /// <summary>Attempts to record the argument of a named parameter.</summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="argument">The argument of the parameter.</param>
    /// <param name="syntax">The <see cref="ExpressionSyntax"/>, syntactically describing the argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract bool TryRecordNamedArgument(string parameterName, object? argument, ExpressionSyntax syntax);
}

/// <summary>Records the arguments of an attribute, together with syntactical information about the arguments.</summary>
/// <typeparam name="TRecord">The type to which the <see cref="IAttributeRecorder"/> records arguments and syntactical information.</typeparam>
public interface IAttributeRecorder<out TRecord> : IAttributeRecorder
{
    /// <summary>Retrieves the <typeparamref name="TRecord"/>, representing the recorded arguments.</summary>
    /// <returns>The <typeparamref name="TRecord"/> representing the recorded arguments.</returns>
    /// <exception cref="InvalidOperationException"/>
    public abstract TRecord GetRecord();
}
