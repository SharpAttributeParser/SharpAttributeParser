namespace SharpAttributeParser.Mappers.Repositories.Semantic;

using Microsoft.CodeAnalysis;

using SharpAttributeParser.Mappers.MappedRecorders;

using System;

/// <inheritdoc cref="IMappedSemanticTypeArgumentRecorderFactory"/>
public sealed class MappedSemanticTypeArgumentRecorderFactory : IMappedSemanticTypeArgumentRecorderFactory
{
    IMappedSemanticTypeArgumentRecorder IMappedSemanticTypeArgumentRecorderFactory.Create<TRecord>(TRecord dataRecord, IDetachedMappedSemanticTypeArgumentRecorder<TRecord> detachedRecorder)
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

    private sealed class Recorder<TRecord> : IMappedSemanticTypeArgumentRecorder
    {
        private readonly TRecord DataRecord;
        private readonly IDetachedMappedSemanticTypeArgumentRecorder<TRecord> DetachedRecorder;

        public Recorder(TRecord dataRecord, IDetachedMappedSemanticTypeArgumentRecorder<TRecord> detachedRecorder)
        {
            DataRecord = dataRecord;
            DetachedRecorder = detachedRecorder;
        }

        bool IMappedSemanticTypeArgumentRecorder.TryRecordArgument(ITypeSymbol argument)
        {
            if (argument is null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            return DetachedRecorder.TryRecordArgument(DataRecord, argument);
        }
    }
}
