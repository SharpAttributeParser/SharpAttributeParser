namespace SharpAttributeParser.Mappers.Repositories.Semantic;

using SharpAttributeParser.Mappers.MappedRecorders;

using System;

/// <inheritdoc cref="IMappedSemanticNamedArgumentRecorderFactory"/>
public sealed class MappedSemanticNamedArgumentRecorderFactory : IMappedSemanticNamedArgumentRecorderFactory
{
    IMappedSemanticNamedArgumentRecorder IMappedSemanticNamedArgumentRecorderFactory.Create<TRecord>(TRecord dataRecord, IDetachedMappedSemanticNamedArgumentRecorder<TRecord> detachedRecorder)
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

    private sealed class Recorder<TRecord> : IMappedSemanticNamedArgumentRecorder
    {
        private TRecord DataRecord { get; }
        private IDetachedMappedSemanticNamedArgumentRecorder<TRecord> DetachedRecorder { get; }

        public Recorder(TRecord dataRecord, IDetachedMappedSemanticNamedArgumentRecorder<TRecord> detachedRecorder)
        {
            DataRecord = dataRecord;
            DetachedRecorder = detachedRecorder;
        }

        bool IMappedSemanticNamedArgumentRecorder.TryRecordArgument(object? argument) => DetachedRecorder.TryRecordArgument(DataRecord, argument);
    }
}
