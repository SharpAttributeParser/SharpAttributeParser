namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

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

        return new SyntacticAttributeRecorder<TRecord, TRecordBuilder<TRecord>>(new TRecordBuilderMapper<TRecord>(argumentMapper), new TRecordBuilder<TRecord>(dataRecord));
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

    private sealed class TRecordBuilder<TRecord> : IRecordBuilder<TRecord>
    {
        public TRecord Target { get; }

        public TRecordBuilder(TRecord target)
        {
            Target = target;
        }

        TRecord IRecordBuilder<TRecord>.Build() => Target;
    }

    private sealed class TRecordBuilderMapper<TRecord> : ISyntacticAttributeMapper<TRecordBuilder<TRecord>>
    {
        private ISyntacticAttributeMapper<TRecord> WrappedMapper { get; }

        public TRecordBuilderMapper(ISyntacticAttributeMapper<TRecord> wrappedMapper)
        {
            WrappedMapper = wrappedMapper;
        }

        ISyntacticAttributeArgumentRecorder? ISyntacticAttributeMapper<TRecordBuilder<TRecord>>.TryMapTypeParameter(ITypeParameterSymbol parameter, TRecordBuilder<TRecord> dataRecord)
        {
            return WrappedMapper.TryMapTypeParameter(parameter, dataRecord.Target);
        }

        ISyntacticAttributeConstructorArgumentRecorder? ISyntacticAttributeMapper<TRecordBuilder<TRecord>>.TryMapConstructorParameter(IParameterSymbol parameter, TRecordBuilder<TRecord> dataRecord)
        {
            return WrappedMapper.TryMapConstructorParameter(parameter, dataRecord.Target);
        }

        ISyntacticAttributeArgumentRecorder? ISyntacticAttributeMapper<TRecordBuilder<TRecord>>.TryMapNamedParameter(string parameterName, TRecordBuilder<TRecord> dataRecord)
        {
            return WrappedMapper.TryMapNamedParameter(parameterName, dataRecord.Target);
        }
    }
}
