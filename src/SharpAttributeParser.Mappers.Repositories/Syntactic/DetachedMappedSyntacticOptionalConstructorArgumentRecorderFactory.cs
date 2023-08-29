namespace SharpAttributeParser.Mappers.Repositories.Syntactic;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;
using OneOf.Types;

using System;
using System.Collections.Generic;

/// <inheritdoc cref="IDetachedMappedSyntacticOptionalConstructorArgumentRecorderFactory{TRecord}"/>
public sealed class DetachedMappedSyntacticOptionalConstructorArgumentRecorderFactory<TRecord> : IDetachedMappedSyntacticOptionalConstructorArgumentRecorderFactory<TRecord>
{
    IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord> IDetachedMappedSyntacticOptionalConstructorArgumentRecorderFactory<TRecord>.Create(Func<TRecord, OneOf<None, ExpressionSyntax>, bool> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return Create(recorder);
    }

    IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord> IDetachedMappedSyntacticOptionalConstructorArgumentRecorderFactory<TRecord>.Create(Action<TRecord, OneOf<None, ExpressionSyntax>> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return Create(recorder.TrueReturning());
    }

    private IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord> Create(Func<TRecord, OneOf<None, ExpressionSyntax>, bool> recorder) => new ArgumentRecorder(recorder);

    private sealed class ArgumentRecorder : IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord>
    {
        private Func<TRecord, OneOf<None, ExpressionSyntax>, bool> Recorder { get; }

        public ArgumentRecorder(Func<TRecord, OneOf<None, ExpressionSyntax>, bool> recorder)
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

        bool IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord>.TryRecordDefaultArgument(TRecord dataRecord)
        {
            if (dataRecord is null)
            {
                throw new ArgumentNullException(nameof(dataRecord));
            }

            return Recorder(dataRecord, new None());
        }
    }
}
