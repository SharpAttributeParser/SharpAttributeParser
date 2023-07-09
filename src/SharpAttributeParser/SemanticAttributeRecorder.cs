namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System;

/// <inheritdoc cref="ISemanticAttributeRecorder{TRecord}"/>
internal sealed class SemanticAttributeRecorder<TRecord> : ISemanticAttributeRecorder<TRecord>
{
    private ISemanticAttributeMapper<TRecord> ArgumentMapper { get; }
    private TRecord DataRecord { get; }

    /// <summary>Instantiates a <see cref="SemanticAttributeRecorder{TRecord}"/>, recording the arguments of an attribute.</summary>
    /// <param name="argumentMapper"><inheritdoc cref="ISemanticAttributeMapper{TRecord}" path="/summary"/></param>
    /// <param name="attributeData">The <typeparamref name="TRecord"/> to which the arguments are recorded.</param>
    /// <exception cref="ArgumentNullException"/>
    public SemanticAttributeRecorder(ISemanticAttributeMapper<TRecord> argumentMapper, TRecord attributeData)
    {
        ArgumentMapper = argumentMapper ?? throw new ArgumentNullException(nameof(argumentMapper));
        DataRecord = attributeData ?? throw new ArgumentNullException(nameof(attributeData));
    }

    /// <inheritdoc/>
    public TRecord GetRecord() => DataRecord;

    /// <inheritdoc/>
    public bool TryRecordTypeArgument(ITypeParameterSymbol parameter, ITypeSymbol argument)
    {
        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        if (argument is null)
        {
            throw new ArgumentNullException(nameof(argument));
        }

        if (ArgumentMapper.TryMapTypeParameter(parameter, DataRecord) is not ISemanticAttributeArgumentRecorder argumentRecorder)
        {
            return false;
        }

        return argumentRecorder.RecordArgument(argument);
    }

    /// <inheritdoc/>
    public bool TryRecordConstructorArgument(IParameterSymbol parameter, object? argument)
    {
        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        if (ArgumentMapper.TryMapConstructorParameter(parameter, DataRecord) is not ISemanticAttributeArgumentRecorder argumentRecorder)
        {
            return false;
        }

        return argumentRecorder.RecordArgument(argument);
    }

    /// <inheritdoc/>
    public bool TryRecordNamedArgument(string parameterName, object? argument)
    {
        if (parameterName is null)
        {
            throw new ArgumentNullException(nameof(parameterName));
        }

        if (ArgumentMapper.TryMapNamedParameter(parameterName, DataRecord) is not ISemanticAttributeArgumentRecorder argumentRecorder)
        {
            return false;
        }

        return argumentRecorder.RecordArgument(argument);
    }
}

/// <summary><inheritdoc cref="ISemanticAttributeRecorder{TRecord}" path="/summary"/></summary>
/// <typeparam name="TRecord">The type representing the recorded attribute arguments, when built by a <typeparamref name="TRecordBuilder"/>.</typeparam>
/// <typeparam name="TRecordBuilder">The type to which the arguments are recorded, and which can build a <typeparamref name="TRecord"/>.</typeparam>
internal sealed class SemanticAttributeRecorder<TRecord, TRecordBuilder> : ISemanticAttributeRecorder<TRecord> where TRecordBuilder : IRecordBuilder<TRecord>
{
    private ISemanticAttributeMapper<TRecordBuilder> ArgumentMapper { get; }
    private TRecordBuilder AttributeDataBuilder { get; }

    /// <summary>Instantiates a <see cref="SemanticAttributeRecorder{TRecord, TRecordBuilder}"/>, recording the parsed of an attribute.</summary>
    /// <param name="argumentMapper"><inheritdoc cref="ISemanticAttributeMapper{TRecord}" path="/summary"/></param>
    /// <param name="attributeDataBuilder">The <typeparamref name="TRecordBuilder"/> to which the produced <see cref="ISemanticAttributeRecorder"/> records attribute arguments, and which can build a <typeparamref name="TRecord"/>.</param>
    /// <exception cref="ArgumentNullException"/>
    public SemanticAttributeRecorder(ISemanticAttributeMapper<TRecordBuilder> argumentMapper, TRecordBuilder attributeDataBuilder)
    {
        ArgumentMapper = argumentMapper ?? throw new ArgumentNullException(nameof(argumentMapper));
        AttributeDataBuilder = attributeDataBuilder ?? throw new ArgumentNullException(nameof(attributeDataBuilder));
    }

    /// <inheritdoc/>
    public TRecord GetRecord() => AttributeDataBuilder.Build();

    /// <inheritdoc/>
    public bool TryRecordTypeArgument(ITypeParameterSymbol parameter, ITypeSymbol argument)
    {
        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        if (argument is null)
        {
            throw new ArgumentNullException(nameof(argument));
        }

        if (ArgumentMapper.TryMapTypeParameter(parameter, AttributeDataBuilder) is not ISemanticAttributeArgumentRecorder argumentRecorder)
        {
            return false;
        }

        return argumentRecorder.RecordArgument(argument);
    }

    /// <inheritdoc/>
    public bool TryRecordConstructorArgument(IParameterSymbol parameter, object? argument)
    {
        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        if (ArgumentMapper.TryMapConstructorParameter(parameter, AttributeDataBuilder) is not ISemanticAttributeArgumentRecorder argumentRecorder)
        {
            return false;
        }

        return argumentRecorder.RecordArgument(argument);
    }

    /// <inheritdoc/>
    public bool TryRecordNamedArgument(string parameterName, object? argument)
    {
        if (parameterName is null)
        {
            throw new ArgumentNullException(nameof(parameterName));
        }

        if (ArgumentMapper.TryMapNamedParameter(parameterName, AttributeDataBuilder) is not ISemanticAttributeArgumentRecorder argumentRecorder)
        {
            return false;
        }

        return argumentRecorder.RecordArgument(argument);
    }
}
