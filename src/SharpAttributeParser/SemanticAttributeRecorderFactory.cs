namespace SharpAttributeParser;

using System;

/// <inheritdoc cref="ISemanticAttributeRecorderFactory"/>
public sealed class SemanticAttributeRecorderFactory : ISemanticAttributeRecorderFactory
{
    /// <inheritdoc/>
    public ISemanticAttributeRecorder<TRecord> Create<TRecord>(ISemanticAttributeMapper<TRecord> argumentMapper, TRecord dataRecord)
    {
        if (argumentMapper is null)
        {
            throw new ArgumentNullException(nameof(argumentMapper));
        }

        if (dataRecord is null)
        {
            throw new ArgumentNullException(nameof(dataRecord));
        }

        return new SemanticAttributeRecorder<TRecord>(argumentMapper, dataRecord);
    }

    /// <inheritdoc/>
    public ISemanticAttributeRecorder<TRecord> Create<TRecord, TRecordBuilder>(ISemanticAttributeMapper<TRecordBuilder> argumentMapper, TRecordBuilder recordBuilder) where TRecordBuilder : IRecordBuilder<TRecord>
    {
        if (argumentMapper is null)
        {
            throw new ArgumentNullException(nameof(argumentMapper));
        }

        if (recordBuilder is null)
        {
            throw new ArgumentNullException(nameof(recordBuilder));
        }

        return new SemanticAttributeRecorder<TRecord, TRecordBuilder>(argumentMapper, recordBuilder);
    }
}
