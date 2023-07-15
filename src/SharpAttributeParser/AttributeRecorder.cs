namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;

/// <summary><inheritdoc cref="IAttributeRecorder{TRecord}" path="/summary"/></summary>
/// <typeparam name="TRecord">The type representing the recorded attribute arguments, when built by a <typeparamref name="TRecordBuilder"/>.</typeparam>
/// <typeparam name="TRecordBuilder">The type to which the arguments are recorded, and which can build a <typeparamref name="TRecord"/>.</typeparam>
internal sealed class AttributeRecorder<TRecord, TRecordBuilder> : IAttributeRecorder<TRecord> where TRecordBuilder : IRecordBuilder<TRecord>
{
    private IAttributeMapper<TRecordBuilder> ArgumentMapper { get; }
    private TRecordBuilder RecordBuilder { get; }

    /// <summary>Instantiates a <see cref="AttributeRecorder{TRecord, TRecordBuilder}"/>, recording the parsed of an attribute.</summary>
    /// <param name="argumentMapper"><inheritdoc cref="IAttributeMapper{TRecord}" path="/summary"/></param>
    /// <param name="recordBuilder">The <typeparamref name="TRecordBuilder"/> to which the produced <see cref="IAttributeRecorder"/> records attribute arguments, and which can build a <typeparamref name="TRecord"/>.</param>
    /// <exception cref="ArgumentNullException"/>
    public AttributeRecorder(IAttributeMapper<TRecordBuilder> argumentMapper, TRecordBuilder recordBuilder)
    {
        ArgumentMapper = argumentMapper ?? throw new ArgumentNullException(nameof(argumentMapper));
        RecordBuilder = recordBuilder ?? throw new ArgumentNullException(nameof(recordBuilder));
    }

    TRecord IAttributeRecorder<TRecord>.GetRecord() => RecordBuilder.Build();

    bool IAttributeRecorder.TryRecordTypeArgument(ITypeParameterSymbol parameter, ITypeSymbol argument, ExpressionSyntax syntax)
    {
        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        if (argument is null)
        {
            throw new ArgumentNullException(nameof(argument));
        }

        if (syntax is null)
        {
            throw new ArgumentNullException(nameof(syntax));
        }

        if (ArgumentMapper.TryMapTypeParameter(parameter, RecordBuilder) is not IAttributeArgumentRecorder argumentRecorder)
        {
            return false;
        }

        return argumentRecorder.RecordArgument(argument, syntax);
    }

    bool IAttributeRecorder.TryRecordConstructorArgument(IParameterSymbol parameter, object? argument, ExpressionSyntax syntax)
    {
        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        if (syntax is null)
        {
            throw new ArgumentNullException(nameof(syntax));
        }

        if (ArgumentMapper.TryMapConstructorParameter(parameter, RecordBuilder) is not IAttributeConstructorArgumentRecorder argumentRecorder)
        {
            return false;
        }

        return argumentRecorder.RecordArgument(argument, syntax);
    }

    bool IAttributeRecorder.TryRecordConstructorParamsArgument(IParameterSymbol parameter, object? argument, IReadOnlyList<ExpressionSyntax> elementSyntax)
    {
        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        if (elementSyntax is null)
        {
            throw new ArgumentNullException(nameof(elementSyntax));
        }

        if (ArgumentMapper.TryMapConstructorParameter(parameter, RecordBuilder) is not IAttributeConstructorArgumentRecorder argumentRecorder)
        {
            return false;
        }

        return argumentRecorder.RecordParamsArgument(argument, elementSyntax);
    }

    bool IAttributeRecorder.TryRecordNamedArgument(string parameterName, object? argument, ExpressionSyntax syntax)
    {
        if (parameterName is null)
        {
            throw new ArgumentNullException(nameof(parameterName));
        }

        if (syntax is null)
        {
            throw new ArgumentNullException(nameof(syntax));
        }

        if (ArgumentMapper.TryMapNamedParameter(parameterName, RecordBuilder) is not IAttributeArgumentRecorder argumentRecorder)
        {
            return false;
        }

        return argumentRecorder.RecordArgument(argument, syntax);
    }
}
