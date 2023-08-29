namespace SharpAttributeParser.Mappers.Repositories.Syntactic;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;

/// <inheritdoc cref="IDetachedMappedSyntacticNamedArgumentRecorderFactory{TRecord}"/>
public sealed class DetachedMappedSyntacticNamedArgumentRecorderFactory<TRecord> : IDetachedMappedSyntacticNamedArgumentRecorderFactory<TRecord>
{
    IDetachedMappedSyntacticNamedArgumentRecorder<TRecord> IDetachedMappedSyntacticNamedArgumentRecorderFactory<TRecord>.Create(Func<TRecord, ExpressionSyntax, bool> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return Create(recorder);
    }

    IDetachedMappedSyntacticNamedArgumentRecorder<TRecord> IDetachedMappedSyntacticNamedArgumentRecorderFactory<TRecord>.Create(Action<TRecord, ExpressionSyntax> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return Create(recorder.TrueReturning());
    }

    private IDetachedMappedSyntacticNamedArgumentRecorder<TRecord> Create(Func<TRecord, ExpressionSyntax, bool> recorder) => new ArgumentRecorder(recorder);

    private sealed class ArgumentRecorder : IDetachedMappedSyntacticNamedArgumentRecorder<TRecord>
    {
        private Func<TRecord, ExpressionSyntax, bool> Recorder { get; }

        public ArgumentRecorder(Func<TRecord, ExpressionSyntax, bool> recorder)
        {
            Recorder = recorder;
        }

        bool IDetachedMappedSyntacticNamedArgumentRecorder<TRecord>.TryRecordArgument(TRecord dataRecord, ExpressionSyntax syntax)
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
    }
}
