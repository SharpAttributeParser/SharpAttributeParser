namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System;

/// <inheritdoc cref="ISemanticAttributeRecorder"/>
/// <typeparam name="TData">The type to which the <see cref="ISemanticAttributeRecorder"/> records attribute arguments.</typeparam>
public sealed class SemanticAttributeRecorder<TData> : ISemanticAttributeRecorder
{
    private ISemanticAttributeMapper<TData> Mapper { get; }
    private TData Data { get; }

    /// <summary>Instantiates a <see cref="SemanticAttributeRecorder{TData}"/>, recording the parsed of an attribute.</summary>
    /// <param name="mapper"><inheritdoc cref="ISemanticAttributeMapper{TData}" path="/summary"/></param>
    /// <param name="data">The <typeparamref name="TData"/> representing the recorded arguments.</param>
    /// <exception cref="ArgumentNullException"/>
    public SemanticAttributeRecorder(ISemanticAttributeMapper<TData> mapper, TData data)
    {
        Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        Data = data ?? throw new ArgumentNullException(nameof(data));
    }

    /// <inheritdoc/>
    public bool TryRecordConstructorArgument(IParameterSymbol parameter, object? value)
    {
        if (Mapper.TryMapConstructorParameter(Data, parameter) is not DSemanticAttributeArgumentRecorder recorderDelegate)
        {
            return false;
        }

        return recorderDelegate(value);
    }

    /// <inheritdoc/>
    public bool TryRecordNamedArgument(string parameterName, object? value)
    {
        if (Mapper.TryMapNamedParameter(Data, parameterName) is not DSemanticAttributeArgumentRecorder recorderDelegate)
        {
            return false;
        }

        return recorderDelegate(value);
    }

    /// <inheritdoc/>
    public bool TryRecordTypeArgument(ITypeParameterSymbol parameter, ITypeSymbol value)
    {
        if (Mapper.TryMapTypeParameter(Data, parameter) is not DSemanticAttributeArgumentRecorder recordDelegate)
        {
            return false;
        }

        return recordDelegate(value);
    }
}
