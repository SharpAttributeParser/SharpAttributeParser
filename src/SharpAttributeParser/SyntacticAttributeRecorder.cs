namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;

/// <inheritdoc cref="ISyntacticAttributeRecorder{TRecord}"/>
internal sealed class SyntacticAttributeRecorder<TRecord> : ISyntacticAttributeRecorder<TRecord>
{
    private ISyntacticAttributeMapper<TRecord> ArgumentMapper { get; }
    private TRecord DataRecord { get; }

    /// <summary>Instantiates a <see cref="SyntacticAttributeRecorder{TRecord}"/>, recording syntactical information about the arguments of an attribute.</summary>
    /// <param name="argumentMapper"><inheritdoc cref="ISyntacticAttributeMapper{TRecord}" path="/summary"/></param>
    /// <param name="attributeData">The <typeparamref name="TRecord"/> to which the syntactial information is recorded.</param>
    /// <exception cref="ArgumentNullException"/>
    public SyntacticAttributeRecorder(ISyntacticAttributeMapper<TRecord> argumentMapper, TRecord attributeData)
    {
        ArgumentMapper = argumentMapper ?? throw new ArgumentNullException(nameof(argumentMapper));
        DataRecord = attributeData ?? throw new ArgumentNullException(nameof(attributeData));
    }

    /// <inheritdoc/>
    public TRecord GetRecord() => DataRecord;

    /// <inheritdoc/>
    public bool TryRecordTypeArgumentSyntax(ITypeParameterSymbol parameter, ExpressionSyntax syntax)
    {
        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        if (syntax is null)
        {
            throw new ArgumentNullException(nameof(syntax));
        }

        if (ArgumentMapper.TryMapTypeParameter(parameter, DataRecord) is not ISyntacticAttributeArgumentRecorder argumentRecorder)
        {
            return false;
        }

        return argumentRecorder.RecordArgumentSyntax(syntax);
    }

    /// <inheritdoc/>
    public bool TryRecordConstructorArgumentSyntax(IParameterSymbol parameter, ExpressionSyntax syntax)
    {
        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        if (syntax is null)
        {
            throw new ArgumentNullException(nameof(syntax));
        }

        if (ArgumentMapper.TryMapConstructorParameter(parameter, DataRecord) is not ISyntacticAttributeArgumentRecorder argumentRecorder)
        {
            return false;
        }

        return argumentRecorder.RecordArgumentSyntax(syntax);
    }

    /// <inheritdoc/>
    public bool TryRecordConstructorParamsArgumentSyntax(IParameterSymbol parameter, IReadOnlyList<ExpressionSyntax> elementSyntax)
    {
        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        if (elementSyntax is null)
        {
            throw new ArgumentNullException(nameof(elementSyntax));
        }

        if (ArgumentMapper.TryMapConstructorParameter(parameter, DataRecord) is not ISyntacticAttributeArgumentRecorder argumentRecorder)
        {
            return false;
        }

        return argumentRecorder.RecordParamsArgumentSyntax(elementSyntax);
    }

    /// <inheritdoc/>
    public bool TryRecordNamedArgumentSyntax(string parameterName, ExpressionSyntax syntax)
    {
        if (parameterName is null)
        {
            throw new ArgumentNullException(nameof(parameterName));
        }

        if (syntax is null)
        {
            throw new ArgumentNullException(nameof(syntax));
        }

        if (ArgumentMapper.TryMapNamedParameter(parameterName, DataRecord) is not ISyntacticAttributeArgumentRecorder argumentRecorder)
        {
            return false;
        }

        return argumentRecorder.RecordArgumentSyntax(syntax);
    }
}

/// <summary><inheritdoc cref="ISyntacticAttributeRecorder{TRecord}" path="/summary"/></summary>
/// <typeparam name="TRecord">The type representing the recorded attribute arguments, when built by a <typeparamref name="TRecordBuilder"/>.</typeparam>
/// <typeparam name="TRecordBuilder">The type to which the arguments are recorded, and which can build a <typeparamref name="TRecord"/>.</typeparam>
internal sealed class SyntacticAttributeRecorder<TRecord, TRecordBuilder> : ISyntacticAttributeRecorder<TRecord> where TRecordBuilder : IRecordBuilder<TRecord>
{
    private ISyntacticAttributeMapper<TRecordBuilder> ArgumentMapper { get; }
    private TRecordBuilder AttributeDataBuilder { get; }

    /// <summary>Instantiates a <see cref="SyntacticAttributeRecorder{TRecord, TRecordBuilder}"/>, recording the parsed of an attribute.</summary>
    /// <param name="argumentMapper"><inheritdoc cref="ISyntacticAttributeMapper{TRecord}" path="/summary"/></param>
    /// <param name="attributeDataBuilder">The <typeparamref name="TRecordBuilder"/> to which the produced <see cref="ISyntacticAttributeRecorder"/> records attribute arguments, and which can build a <typeparamref name="TRecord"/>.</param>
    /// <exception cref="ArgumentNullException"/>
    public SyntacticAttributeRecorder(ISyntacticAttributeMapper<TRecordBuilder> argumentMapper, TRecordBuilder attributeDataBuilder)
    {
        ArgumentMapper = argumentMapper ?? throw new ArgumentNullException(nameof(argumentMapper));
        AttributeDataBuilder = attributeDataBuilder ?? throw new ArgumentNullException(nameof(attributeDataBuilder));
    }

    /// <inheritdoc/>
    public TRecord GetRecord() => AttributeDataBuilder.Build();

    /// <inheritdoc/>
    public bool TryRecordTypeArgumentSyntax(ITypeParameterSymbol parameter, ExpressionSyntax syntax)
    {
        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        if (syntax is null)
        {
            throw new ArgumentNullException(nameof(syntax));
        }

        if (ArgumentMapper.TryMapTypeParameter(parameter, AttributeDataBuilder) is not ISyntacticAttributeArgumentRecorder argumentRecorder)
        {
            return false;
        }

        return argumentRecorder.RecordArgumentSyntax(syntax);
    }

    /// <inheritdoc/>
    public bool TryRecordConstructorArgumentSyntax(IParameterSymbol parameter, ExpressionSyntax syntax)
    {
        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        if (syntax is null)
        {
            throw new ArgumentNullException(nameof(syntax));
        }

        if (ArgumentMapper.TryMapConstructorParameter(parameter, AttributeDataBuilder) is not ISyntacticAttributeArgumentRecorder argumentRecorder)
        {
            return false;
        }

        return argumentRecorder.RecordArgumentSyntax(syntax);
    }

    /// <inheritdoc/>
    public bool TryRecordConstructorParamsArgumentSyntax(IParameterSymbol parameter, IReadOnlyList<ExpressionSyntax> elementSyntax)
    {
        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        if (elementSyntax is null)
        {
            throw new ArgumentNullException(nameof(elementSyntax));
        }

        if (ArgumentMapper.TryMapConstructorParameter(parameter, AttributeDataBuilder) is not ISyntacticAttributeArgumentRecorder argumentRecorder)
        {
            return false;
        }

        return argumentRecorder.RecordParamsArgumentSyntax(elementSyntax);
    }

    /// <inheritdoc/>
    public bool TryRecordNamedArgumentSyntax(string parameterName, ExpressionSyntax syntax)
    {
        if (parameterName is null)
        {
            throw new ArgumentNullException(nameof(parameterName));
        }

        if (syntax is null)
        {
            throw new ArgumentNullException(nameof(syntax));
        }

        if (ArgumentMapper.TryMapNamedParameter(parameterName, AttributeDataBuilder) is not ISyntacticAttributeArgumentRecorder argumentRecorder)
        {
            return false;
        }

        return argumentRecorder.RecordArgumentSyntax(syntax);
    }
}
