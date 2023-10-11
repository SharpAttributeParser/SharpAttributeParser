namespace SharpAttributeParser.Mappers.Repositories.Semantic;

using SharpAttributeParser.Mappers.MappedRecorders;

using System;

/// <inheritdoc cref="IMappedSemanticConstructorArgumentRecorderFactory"/>
public sealed class MappedSemanticConstructorArgumentRecorderFactory : IMappedSemanticConstructorArgumentRecorderFactory
{
    IMappedSemanticConstructorArgumentRecorder IMappedSemanticConstructorArgumentRecorderFactory.Create<TRecord>(TRecord dataRecord, IDetachedMappedSemanticConstructorArgumentRecorder<TRecord> detachedRecorder)
    {
        if (dataRecord is null)
        {
            throw new ArgumentNullException(nameof(dataRecord));
        }

        if (detachedRecorder is null)
        {
            throw new ArgumentNullException(nameof(detachedRecorder));
        }

        return new Recorder<TRecord>(dataRecord, detachedRecorder);
    }

    private sealed class Recorder<TRecord> : IMappedSemanticConstructorArgumentRecorder
    {
        private readonly TRecord DataRecord;
        private readonly IDetachedMappedSemanticConstructorArgumentRecorder<TRecord> DetachedRecorder;

        public Recorder(TRecord dataRecord, IDetachedMappedSemanticConstructorArgumentRecorder<TRecord> detachedRecorder)
        {
            DataRecord = dataRecord;
            DetachedRecorder = detachedRecorder;
        }

        bool IMappedSemanticConstructorArgumentRecorder.TryRecordArgument(object? argument) => DetachedRecorder.TryRecordArgument(DataRecord, argument);
    }
}
