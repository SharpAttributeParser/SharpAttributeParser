namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

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

        return new SemanticAttributeRecorder<TRecord, TRecordBuilder<TRecord>>(new TRecordBuilderMapper<TRecord>(argumentMapper), new TRecordBuilder<TRecord>(dataRecord));
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

    private sealed class TRecordBuilder<TRecord> : IRecordBuilder<TRecord>
    {
        public TRecord Target { get; }

        public TRecordBuilder(TRecord target)
        {
            Target = target;
        }

        TRecord IRecordBuilder<TRecord>.Build() => Target;
    }

    private sealed class TRecordBuilderMapper<TRecord> : ISemanticAttributeMapper<TRecordBuilder<TRecord>>
    {
        private ISemanticAttributeMapper<TRecord> WrappedMapper { get; }

        public TRecordBuilderMapper(ISemanticAttributeMapper<TRecord> wrappedMapper)
        {
            WrappedMapper = wrappedMapper;
        }

        ISemanticAttributeArgumentRecorder? ISemanticAttributeMapper<TRecordBuilder<TRecord>>.TryMapTypeParameter(ITypeParameterSymbol parameter, TRecordBuilder<TRecord> dataRecord)
        {
            return WrappedMapper.TryMapTypeParameter(parameter, dataRecord.Target);
        }

        ISemanticAttributeArgumentRecorder? ISemanticAttributeMapper<TRecordBuilder<TRecord>>.TryMapConstructorParameter(IParameterSymbol parameter, TRecordBuilder<TRecord> dataRecord)
        {
            return WrappedMapper.TryMapConstructorParameter(parameter, dataRecord.Target);
        }

        ISemanticAttributeArgumentRecorder? ISemanticAttributeMapper<TRecordBuilder<TRecord>>.TryMapNamedParameter(string parameterName, TRecordBuilder<TRecord> dataRecord)
        {
            return WrappedMapper.TryMapNamedParameter(parameterName, dataRecord.Target);
        }
    }
}
