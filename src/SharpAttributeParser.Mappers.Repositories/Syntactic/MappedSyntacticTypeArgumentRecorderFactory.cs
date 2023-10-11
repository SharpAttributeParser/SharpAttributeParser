namespace SharpAttributeParser.Mappers.Repositories.Syntactic;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser.Mappers.MappedRecorders;

using System;

/// <inheritdoc cref="IMappedSyntacticTypeArgumentRecorderFactory"/>
public sealed class MappedSyntacticTypeArgumentRecorderFactory : IMappedSyntacticTypeArgumentRecorderFactory
{
    IMappedSyntacticTypeArgumentRecorder IMappedSyntacticTypeArgumentRecorderFactory.Create<TRecord>(TRecord dataRecord, IDetachedMappedSyntacticTypeArgumentRecorder<TRecord> detachedRecorder)
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

    private sealed class Recorder<TRecord> : IMappedSyntacticTypeArgumentRecorder
    {
        private readonly TRecord DataRecord;
        private readonly IDetachedMappedSyntacticTypeArgumentRecorder<TRecord> DetachedRecorder;

        public Recorder(TRecord dataRecord, IDetachedMappedSyntacticTypeArgumentRecorder<TRecord> detachedRecorder)
        {
            DataRecord = dataRecord;
            DetachedRecorder = detachedRecorder;
        }

        bool IMappedSyntacticTypeArgumentRecorder.TryRecordArgument(ExpressionSyntax syntax)
        {
            if (syntax is null)
            {
                throw new ArgumentNullException(nameof(syntax));
            }

            return DetachedRecorder.TryRecordArgument(DataRecord, syntax);
        }
    }
}
