namespace SharpAttributeParser.Mappers.Repositories.Combined;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser.Mappers.MappedRecorders;

using System;

/// <inheritdoc cref="IMappedCombinedNamedArgumentRecorderFactory"/>
public sealed class MappedCombinedNamedArgumentRecorderFactory : IMappedCombinedNamedArgumentRecorderFactory
{
    IMappedCombinedNamedArgumentRecorder IMappedCombinedNamedArgumentRecorderFactory.Create<TRecord>(TRecord dataRecord, IDetachedMappedCombinedNamedArgumentRecorder<TRecord> detachedRecorder)
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

    private sealed class Recorder<TRecord> : IMappedCombinedNamedArgumentRecorder
    {
        private TRecord DataRecord { get; }
        private IDetachedMappedCombinedNamedArgumentRecorder<TRecord> DetachedRecorder { get; }

        public Recorder(TRecord dataRecord, IDetachedMappedCombinedNamedArgumentRecorder<TRecord> detachedRecorder)
        {
            DataRecord = dataRecord;
            DetachedRecorder = detachedRecorder;
        }

        bool IMappedCombinedNamedArgumentRecorder.TryRecordArgument(object? argument, ExpressionSyntax syntax)
        {
            if (syntax is null)
            {
                throw new ArgumentNullException(nameof(syntax));
            }

            return DetachedRecorder.TryRecordArgument(DataRecord, argument, syntax);
        }
    }
}
