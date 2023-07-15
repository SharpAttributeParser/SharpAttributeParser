namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System;

/// <summary><inheritdoc cref="ISemanticAttributeRecorder{TRecord}" path="/summary"/></summary>
/// <typeparam name="TRecord">The type representing the recorded attribute arguments, when built by a <typeparamref name="TRecordBuilder"/>.</typeparam>
/// <typeparam name="TRecordBuilder">The type to which the arguments are recorded, and which can build a <typeparamref name="TRecord"/>.</typeparam>
internal sealed class SemanticAttributeRecorder<TRecord, TRecordBuilder> : ISemanticAttributeRecorder<TRecord> where TRecordBuilder : IRecordBuilder<TRecord>
{
    private ISemanticAttributeMapper<TRecordBuilder> ArgumentMapper { get; }
    private TRecordBuilder RecordBuilder { get; }

    /// <summary>Instantiates a <see cref="SemanticAttributeRecorder{TRecord, TRecordBuilder}"/>, recording the parsed of an attribute.</summary>
    /// <param name="argumentMapper"><inheritdoc cref="ISemanticAttributeMapper{TRecord}" path="/summary"/></param>
    /// <param name="recordBuilder">The <typeparamref name="TRecordBuilder"/> to which the produced <see cref="ISemanticAttributeRecorder"/> records attribute arguments, and which can build a <typeparamref name="TRecord"/>.</param>
    /// <exception cref="ArgumentNullException"/>
    public SemanticAttributeRecorder(ISemanticAttributeMapper<TRecordBuilder> argumentMapper, TRecordBuilder recordBuilder)
    {
        ArgumentMapper = argumentMapper ?? throw new ArgumentNullException(nameof(argumentMapper));
        RecordBuilder = recordBuilder ?? throw new ArgumentNullException(nameof(recordBuilder));
    }

    TRecord ISemanticAttributeRecorder<TRecord>.GetRecord() => RecordBuilder.Build();

    bool ISemanticAttributeRecorder.TryRecordTypeArgument(ITypeParameterSymbol parameter, ITypeSymbol argument)
    {
        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        if (argument is null)
        {
            throw new ArgumentNullException(nameof(argument));
        }

        if (ArgumentMapper.TryMapTypeParameter(parameter, RecordBuilder) is not ISemanticAttributeArgumentRecorder argumentRecorder)
        {
            return false;
        }

        return argumentRecorder.RecordArgument(argument);
    }

    bool ISemanticAttributeRecorder.TryRecordConstructorArgument(IParameterSymbol parameter, object? argument)
    {
        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        if (ArgumentMapper.TryMapConstructorParameter(parameter, RecordBuilder) is not ISemanticAttributeArgumentRecorder argumentRecorder)
        {
            return false;
        }

        return argumentRecorder.RecordArgument(argument);
    }

    bool ISemanticAttributeRecorder.TryRecordNamedArgument(string parameterName, object? argument)
    {
        if (parameterName is null)
        {
            throw new ArgumentNullException(nameof(parameterName));
        }

        if (ArgumentMapper.TryMapNamedParameter(parameterName, RecordBuilder) is not ISemanticAttributeArgumentRecorder argumentRecorder)
        {
            return false;
        }

        return argumentRecorder.RecordArgument(argument);
    }
}
