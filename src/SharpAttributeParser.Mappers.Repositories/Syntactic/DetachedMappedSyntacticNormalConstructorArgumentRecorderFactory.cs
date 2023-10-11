namespace SharpAttributeParser.Mappers.Repositories.Syntactic;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;

/// <inheritdoc cref="IDetachedMappedSyntacticNormalConstructorArgumentRecorderFactory{TRecord}"/>
public sealed class DetachedMappedSyntacticNormalConstructorArgumentRecorderFactory<TRecord> : IDetachedMappedSyntacticNormalConstructorArgumentRecorderFactory<TRecord>
{
    IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord> IDetachedMappedSyntacticNormalConstructorArgumentRecorderFactory<TRecord>.Create(Func<TRecord, ExpressionSyntax, bool> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return Create(recorder);
    }

    IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord> IDetachedMappedSyntacticNormalConstructorArgumentRecorderFactory<TRecord>.Create(Action<TRecord, ExpressionSyntax> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return Create(recorder.TrueReturning());
    }

    private IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord> Create(Func<TRecord, ExpressionSyntax, bool> recorder) => new ArgumentRecorder(recorder);

    private sealed class ArgumentRecorder : IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord>
    {
        private readonly Func<TRecord, ExpressionSyntax, bool> Recorder;

        public ArgumentRecorder(Func<TRecord, ExpressionSyntax, bool> recorder)
        {
            Recorder = recorder;
        }

        bool IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord>.TryRecordArgument(TRecord dataRecord, ExpressionSyntax syntax)
        {
            if (dataRecord is null)
            {
                throw new ArgumentNullException(nameof(dataRecord));
            }

            if (syntax is null)
            {
                throw new ArgumentNullException(nameof(syntax));
            }

            return Recorder(dataRecord, syntax);
        }

        bool IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord>.TryRecordParamsArgument(TRecord dataRecord, IReadOnlyList<ExpressionSyntax> elementSyntax) => false;
        bool IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord>.TryRecordDefaultArgument(TRecord dataRecord) => false;
    }
}
