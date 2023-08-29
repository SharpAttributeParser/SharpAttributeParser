namespace SharpAttributeParser.Mappers.Repositories.Syntactic;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser.Mappers.MappedRecorders;

using System;

/// <inheritdoc cref="IMappedSyntacticNamedArgumentRecorderFactory"/>
public sealed class MappedSyntacticNamedArgumentRecorderFactory : IMappedSyntacticNamedArgumentRecorderFactory
{
    IMappedSyntacticNamedArgumentRecorder IMappedSyntacticNamedArgumentRecorderFactory.Create<TRecord>(TRecord dataRecord, IDetachedMappedSyntacticNamedArgumentRecorder<TRecord> detachedRecorder)
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

    private sealed class Recorder<TRecord> : IMappedSyntacticNamedArgumentRecorder
    {
        private TRecord DataRecord { get; }
        private IDetachedMappedSyntacticNamedArgumentRecorder<TRecord> DetachedRecorder { get; }

        public Recorder(TRecord dataRecord, IDetachedMappedSyntacticNamedArgumentRecorder<TRecord> detachedRecorder)
        {
            DataRecord = dataRecord;
            DetachedRecorder = detachedRecorder;
        }

        bool IMappedSyntacticNamedArgumentRecorder.TryRecordArgument(ExpressionSyntax syntax)
        {
            if (syntax is null)
            {
                throw new ArgumentNullException(nameof(syntax));
            }

            return DetachedRecorder.TryRecordArgument(DataRecord, syntax);
        }
    }
}
