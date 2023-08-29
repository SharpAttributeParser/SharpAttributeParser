namespace SharpAttributeParser.Mappers.Repositories.Combined;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser.Mappers.MappedRecorders;

using System;

/// <inheritdoc cref="IMappedCombinedTypeArgumentRecorderFactory"/>
public sealed class MappedCombinedTypeArgumentRecorderFactory : IMappedCombinedTypeArgumentRecorderFactory
{
    IMappedCombinedTypeArgumentRecorder IMappedCombinedTypeArgumentRecorderFactory.Create<TRecord>(TRecord dataRecord, IDetachedMappedCombinedTypeArgumentRecorder<TRecord> detachedRecorder)
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

    private sealed class Recorder<TRecord> : IMappedCombinedTypeArgumentRecorder
    {
        private TRecord DataRecord { get; }
        private IDetachedMappedCombinedTypeArgumentRecorder<TRecord> DetachedRecorder { get; }

        public Recorder(TRecord dataRecord, IDetachedMappedCombinedTypeArgumentRecorder<TRecord> detachedRecorder)
        {
            DataRecord = dataRecord;
            DetachedRecorder = detachedRecorder;
        }

        bool IMappedCombinedTypeArgumentRecorder.TryRecordArgument(ITypeSymbol argument, ExpressionSyntax syntax)
        {
            if (argument is null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            if (syntax is null)
            {
                throw new ArgumentNullException(nameof(syntax));
            }

            return DetachedRecorder.TryRecordArgument(DataRecord, argument, syntax);
        }
    }
}
