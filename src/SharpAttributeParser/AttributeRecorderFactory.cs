namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System;

/// <inheritdoc cref="IAttributeRecorderFactory"/>
public sealed class AttributeRecorderFactory : IAttributeRecorderFactory
{
    /// <inheritdoc/>
    public IAttributeRecorder<TRecord> Create<TRecord>(IAttributeMapper<TRecord> argumentMapper, TRecord dataRecord)
    {
        if (argumentMapper is null)
        {
            throw new ArgumentNullException(nameof(argumentMapper));
        }

        if (dataRecord is null)
        {
            throw new ArgumentNullException(nameof(dataRecord));
        }

        return new AttributeRecorder<TRecord, TRecordBuilder<TRecord>>(new TRecordBuilderMapper<TRecord>(argumentMapper), new TRecordBuilder<TRecord>(dataRecord));
    }

    /// <inheritdoc/>
    public IAttributeRecorder<TRecord> Create<TRecord, TRecordBuilder>(IAttributeMapper<TRecordBuilder> argumentMapper, TRecordBuilder recordBuilder) where TRecordBuilder : IRecordBuilder<TRecord>
    {
        if (argumentMapper is null)
        {
            throw new ArgumentNullException(nameof(argumentMapper));
        }

        if (recordBuilder is null)
        {
            throw new ArgumentNullException(nameof(recordBuilder));
        }

        return new AttributeRecorder<TRecord, TRecordBuilder>(argumentMapper, recordBuilder);
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

    private sealed class TRecordBuilderMapper<TRecord> : IAttributeMapper<TRecordBuilder<TRecord>>
    {
        private IAttributeMapper<TRecord> WrappedMapper { get; }

        public TRecordBuilderMapper(IAttributeMapper<TRecord> wrappedMapper)
        {
            WrappedMapper = wrappedMapper;
        }

        IAttributeArgumentRecorder? IAttributeMapper<TRecordBuilder<TRecord>>.TryMapTypeParameter(ITypeParameterSymbol parameter, TRecordBuilder<TRecord> dataRecord)
        {
            return WrappedMapper.TryMapTypeParameter(parameter, dataRecord.Target);
        }

        IAttributeConstructorArgumentRecorder? IAttributeMapper<TRecordBuilder<TRecord>>.TryMapConstructorParameter(IParameterSymbol parameter, TRecordBuilder<TRecord> dataRecord)
        {
            return WrappedMapper.TryMapConstructorParameter(parameter, dataRecord.Target);
        }

        IAttributeArgumentRecorder? IAttributeMapper<TRecordBuilder<TRecord>>.TryMapNamedParameter(string parameterName, TRecordBuilder<TRecord> dataRecord)
        {
            return WrappedMapper.TryMapNamedParameter(parameterName, dataRecord.Target);
        }
    }
}
