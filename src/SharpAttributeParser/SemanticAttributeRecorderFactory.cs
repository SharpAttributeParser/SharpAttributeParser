namespace SharpAttributeParser;

using System;

/// <inheritdoc cref="ISemanticAttributeRecorderFactory"/>
public sealed class SemanticAttributeRecorderFactory : ISemanticAttributeRecorderFactory
{
    /// <inheritdoc/>
    public ISemanticAttributeRecorder<TData> Create<TData>(ISemanticAttributeMapper<TData> argumentMapper, TData dataRecord)
    {
        if (argumentMapper is null)
        {
            throw new ArgumentNullException(nameof(argumentMapper));
        }

        if (dataRecord is null)
        {
            throw new ArgumentNullException(nameof(dataRecord));
        }

        return new SemanticAttributeRecorder<TData>(argumentMapper, dataRecord);
    }

    /// <inheritdoc/>
    public ISemanticAttributeRecorder<TData> Create<TData, TDataBuilder>(ISemanticAttributeMapper<TDataBuilder> argumentMapper, TDataBuilder dataBuilder) where TDataBuilder : IAttributeDataBuilder<TData>
    {
        if (argumentMapper is null)
        {
            throw new ArgumentNullException(nameof(argumentMapper));
        }

        if (dataBuilder is null)
        {
            throw new ArgumentNullException(nameof(dataBuilder));
        }

        return new SemanticAttributeRecorder<TData, TDataBuilder>(argumentMapper, dataBuilder);
    }
}
