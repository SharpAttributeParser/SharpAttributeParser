namespace SharpAttributeParser;

using System;

/// <inheritdoc cref="ISyntacticAttributeRecorderFactory"/>
public sealed class SyntacticAttributeRecorderFactory : ISyntacticAttributeRecorderFactory
{
    /// <inheritdoc/>
    public ISyntacticAttributeRecorder<TRecord> Create<TRecord>(ISyntacticAttributeMapper<TRecord> argumentMapper, TRecord dataRecord)
    {
        if (argumentMapper is null)
        {
            throw new ArgumentNullException(nameof(argumentMapper));
        }

        if (dataRecord is null)
        {
            throw new ArgumentNullException(nameof(dataRecord));
        }

        return new SyntacticAttributeRecorder<TRecord>(argumentMapper, dataRecord);
    }

    /// <inheritdoc/>
    public ISyntacticAttributeRecorder<TRecord> Create<TRecord, TRecordBuilder>(ISyntacticAttributeMapper<TRecordBuilder> argumentMapper, TRecordBuilder recordBuilder) where TRecordBuilder : IRecordBuilder<TRecord>
    {
        if (argumentMapper is null)
        {
            throw new ArgumentNullException(nameof(argumentMapper));
        }

        if (recordBuilder is null)
        {
            throw new ArgumentNullException(nameof(recordBuilder));
        }

        return new SyntacticAttributeRecorder<TRecord, TRecordBuilder>(argumentMapper, recordBuilder);
    }
}
