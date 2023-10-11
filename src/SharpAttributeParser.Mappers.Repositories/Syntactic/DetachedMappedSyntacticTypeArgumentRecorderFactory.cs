namespace SharpAttributeParser.Mappers.Repositories.Syntactic;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;

/// <inheritdoc cref="IDetachedMappedSyntacticTypeArgumentRecorderFactory{TRecord}"/>
public sealed class DetachedMappedSyntacticTypeArgumentRecorderFactory<TRecord> : IDetachedMappedSyntacticTypeArgumentRecorderFactory<TRecord>
{
    IDetachedMappedSyntacticTypeArgumentRecorder<TRecord> IDetachedMappedSyntacticTypeArgumentRecorderFactory<TRecord>.Create(Func<TRecord, ExpressionSyntax, bool> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return Create(recorder);
    }

    IDetachedMappedSyntacticTypeArgumentRecorder<TRecord> IDetachedMappedSyntacticTypeArgumentRecorderFactory<TRecord>.Create(Action<TRecord, ExpressionSyntax> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return Create(recorder.TrueReturning());
    }

    private IDetachedMappedSyntacticTypeArgumentRecorder<TRecord> Create(Func<TRecord, ExpressionSyntax, bool> recorder) => new ArgumentRecorder(recorder);

    private sealed class ArgumentRecorder : IDetachedMappedSyntacticTypeArgumentRecorder<TRecord>
    {
        private readonly Func<TRecord, ExpressionSyntax, bool> Recorder;

        public ArgumentRecorder(Func<TRecord, ExpressionSyntax, bool> recorder)
        {
            Recorder = recorder;
        }

        bool IDetachedMappedSyntacticTypeArgumentRecorder<TRecord>.TryRecordArgument(TRecord dataRecord, ExpressionSyntax syntax)
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
