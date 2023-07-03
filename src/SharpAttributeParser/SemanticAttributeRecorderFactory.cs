namespace SharpAttributeParser;

using System;

/// <inheritdoc cref="ISemanticAttributeRecorderFactory"/>
public sealed class SemanticAttributeRecorderFactory : ISemanticAttributeRecorderFactory
{
    /// <inheritdoc/>
    public ISemanticAttributeRecorder Create<TData>(ISemanticAttributeMapper<TData> argumentMapper, TData data)
    {
        if (argumentMapper is null)
        {
            throw new ArgumentNullException(nameof(argumentMapper));
        }

        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        return new SemanticAttributeRecorder<TData>(argumentMapper, data);
    }
}
