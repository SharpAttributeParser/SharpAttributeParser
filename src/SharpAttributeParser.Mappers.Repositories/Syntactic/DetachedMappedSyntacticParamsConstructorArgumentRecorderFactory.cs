namespace SharpAttributeParser.Mappers.Repositories.Syntactic;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using System;
using System.Collections.Generic;

/// <inheritdoc cref="IDetachedMappedSyntacticParamsConstructorArgumentRecorderFactory{TRecord}"/>
public sealed class DetachedMappedSyntacticParamsConstructorArgumentRecorderFactory<TRecord> : IDetachedMappedSyntacticParamsConstructorArgumentRecorderFactory<TRecord>
{
    IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord> IDetachedMappedSyntacticParamsConstructorArgumentRecorderFactory<TRecord>.Create(Func<TRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return Create(recorder);
    }

    IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord> IDetachedMappedSyntacticParamsConstructorArgumentRecorderFactory<TRecord>.Create(Action<TRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return Create(recorder.TrueReturning());
    }

    private IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord> Create(Func<TRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> recorder) => new ArgumentRecorder(recorder);

    private sealed class ArgumentRecorder : IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord>
    {
        private readonly Func<TRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> Recorder;

        public ArgumentRecorder(Func<TRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> recorder)
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

        bool IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord>.TryRecordParamsArgument(TRecord dataRecord, IReadOnlyList<ExpressionSyntax> elementSyntax)
        {
            if (dataRecord is null)
            {
                throw new ArgumentNullException(nameof(dataRecord));
            }

            if (elementSyntax is null)
            {
                throw new ArgumentNullException(nameof(elementSyntax));
            }

            return Recorder(dataRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(elementSyntax));
        }

        bool IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord>.TryRecordDefaultArgument(TRecord dataRecord) => false;
    }
}
