namespace SharpAttributeParser.Mappers.Repositories.Combined;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser.Mappers.MappedRecorders;

using System;
using System.Collections.Generic;

/// <inheritdoc cref="IMappedCombinedConstructorArgumentRecorderFactory"/>
public sealed class MappedCombinedConstructorArgumentRecorderFactory : IMappedCombinedConstructorArgumentRecorderFactory
{
    IMappedCombinedConstructorArgumentRecorder IMappedCombinedConstructorArgumentRecorderFactory.Create<TRecord>(TRecord dataRecord, IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> detachedRecorder)
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

    private sealed class Recorder<TRecord> : IMappedCombinedConstructorArgumentRecorder
    {
        private TRecord DataRecord { get; }
        private IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> DetachedRecorder { get; }

        public Recorder(TRecord dataRecord, IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> detachedRecorder)
        {
            DataRecord = dataRecord;
            DetachedRecorder = detachedRecorder;
        }

        bool IMappedCombinedConstructorArgumentRecorder.TryRecordArgument(object? argument, ExpressionSyntax syntax)
        {
            if (syntax is null)
            {
                throw new ArgumentNullException(nameof(syntax));
            }

            return DetachedRecorder.TryRecordArgument(DataRecord, argument, syntax);
        }

        bool IMappedCombinedConstructorArgumentRecorder.TryRecordParamsArgument(object? argument, IReadOnlyList<ExpressionSyntax> elementSyntax)
        {
            if (elementSyntax is null)
            {
                throw new ArgumentNullException(nameof(elementSyntax));
            }

            return DetachedRecorder.TryRecordParamsArgument(DataRecord, argument, elementSyntax);
        }

        bool IMappedCombinedConstructorArgumentRecorder.TryRecordDefaultArgument(object? argument) => DetachedRecorder.TryRecordDefaultArgument(DataRecord, argument);
    }
}
