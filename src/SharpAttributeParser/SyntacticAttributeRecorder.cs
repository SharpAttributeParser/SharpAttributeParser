namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;

/// <summary><inheritdoc cref="ISyntacticAttributeRecorder{TRecord}" path="/summary"/></summary>
/// <typeparam name="TRecord">The type representing the recorded attribute arguments, when built by a <typeparamref name="TRecordBuilder"/>.</typeparam>
/// <typeparam name="TRecordBuilder">The type to which the arguments are recorded, and which can build a <typeparamref name="TRecord"/>.</typeparam>
internal sealed class SyntacticAttributeRecorder<TRecord, TRecordBuilder> : ISyntacticAttributeRecorder<TRecord> where TRecordBuilder : IRecordBuilder<TRecord>
{
    private ISyntacticAttributeMapper<TRecordBuilder> ArgumentMapper { get; }
    private TRecordBuilder RecordBuilder { get; }

    /// <summary>Instantiates a <see cref="SyntacticAttributeRecorder{TRecord, TRecordBuilder}"/>, recording the parsed of an attribute.</summary>
    /// <param name="argumentMapper"><inheritdoc cref="ISyntacticAttributeMapper{TRecord}" path="/summary"/></param>
    /// <param name="recordBuilder">The <typeparamref name="TRecordBuilder"/> to which the produced <see cref="ISyntacticAttributeRecorder"/> records attribute arguments, and which can build a <typeparamref name="TRecord"/>.</param>
    /// <exception cref="ArgumentNullException"/>
    public SyntacticAttributeRecorder(ISyntacticAttributeMapper<TRecordBuilder> argumentMapper, TRecordBuilder recordBuilder)
    {
        ArgumentMapper = argumentMapper ?? throw new ArgumentNullException(nameof(argumentMapper));
        RecordBuilder = recordBuilder ?? throw new ArgumentNullException(nameof(recordBuilder));
    }

    TRecord ISyntacticAttributeRecorder<TRecord>.GetRecord() => RecordBuilder.Build();

    bool ISyntacticAttributeRecorder.TryRecordTypeArgumentSyntax(ITypeParameterSymbol parameter, ExpressionSyntax syntax)
    {
        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        if (syntax is null)
        {
            throw new ArgumentNullException(nameof(syntax));
        }

        if (ArgumentMapper.TryMapTypeParameter(parameter, RecordBuilder) is not ISyntacticAttributeArgumentRecorder argumentRecorder)
        {
            return false;
        }

        return argumentRecorder.RecordArgumentSyntax(syntax);
    }

    bool ISyntacticAttributeRecorder.TryRecordConstructorArgumentSyntax(IParameterSymbol parameter, ExpressionSyntax syntax)
    {
        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        if (syntax is null)
        {
            throw new ArgumentNullException(nameof(syntax));
        }

        if (ArgumentMapper.TryMapConstructorParameter(parameter, RecordBuilder) is not ISyntacticAttributeConstructorArgumentRecorder argumentRecorder)
        {
            return false;
        }

        return argumentRecorder.RecordArgumentSyntax(syntax);
    }

    bool ISyntacticAttributeRecorder.TryRecordConstructorParamsArgumentSyntax(IParameterSymbol parameter, IReadOnlyList<ExpressionSyntax> elementSyntax)
    {
        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        if (elementSyntax is null)
        {
            throw new ArgumentNullException(nameof(elementSyntax));
        }

        if (ArgumentMapper.TryMapConstructorParameter(parameter, RecordBuilder) is not ISyntacticAttributeConstructorArgumentRecorder argumentRecorder)
        {
            return false;
        }

        return argumentRecorder.RecordParamsArgumentSyntax(elementSyntax);
    }

    bool ISyntacticAttributeRecorder.TryRecordNamedArgumentSyntax(string parameterName, ExpressionSyntax syntax)
    {
        if (parameterName is null)
        {
            throw new ArgumentNullException(nameof(parameterName));
        }

        if (syntax is null)
        {
            throw new ArgumentNullException(nameof(syntax));
        }

        if (ArgumentMapper.TryMapNamedParameter(parameterName, RecordBuilder) is not ISyntacticAttributeArgumentRecorder argumentRecorder)
        {
            return false;
        }

        return argumentRecorder.RecordArgumentSyntax(syntax);
    }
}
