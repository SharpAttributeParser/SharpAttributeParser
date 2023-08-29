namespace SharpAttributeParser.Mappers.Repositories.Syntactic;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser.Mappers.MappedRecorders;

using System;
using System.Collections.Generic;

/// <inheritdoc cref="IMappedSyntacticConstructorArgumentRecorderFactory"/>
public sealed class MappedSyntacticConstructorArgumentRecorderFactory : IMappedSyntacticConstructorArgumentRecorderFactory
{
    IMappedSyntacticConstructorArgumentRecorder IMappedSyntacticConstructorArgumentRecorderFactory.Create<TRecord>(TRecord dataRecord, IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord> detachedRecorder)
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

    private sealed class Recorder<TRecord> : IMappedSyntacticConstructorArgumentRecorder
    {
        private TRecord DataRecord { get; }
        private IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord> DetachedRecorder { get; }

        public Recorder(TRecord dataRecord, IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord> detachedRecorder)
        {
            DataRecord = dataRecord;
            DetachedRecorder = detachedRecorder;
        }

        bool IMappedSyntacticConstructorArgumentRecorder.TryRecordArgument(ExpressionSyntax syntax)
        {
            if (syntax is null)
            {
                throw new ArgumentNullException(nameof(syntax));
            }

            return DetachedRecorder.TryRecordArgument(DataRecord, syntax);
        }

        bool IMappedSyntacticConstructorArgumentRecorder.TryRecordParamsArgument(IReadOnlyList<ExpressionSyntax> elementSyntax)
        {
            if (elementSyntax is null)
            {
                throw new ArgumentNullException(nameof(elementSyntax));
            }

            return DetachedRecorder.TryRecordParamsArgument(DataRecord, elementSyntax);
        }

        bool IMappedSyntacticConstructorArgumentRecorder.TryRecordDefaultArgument() => DetachedRecorder.TryRecordDefaultArgument(DataRecord);
    }
}
