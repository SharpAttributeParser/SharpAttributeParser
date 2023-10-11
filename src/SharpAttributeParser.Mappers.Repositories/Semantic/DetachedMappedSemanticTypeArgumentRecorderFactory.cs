namespace SharpAttributeParser.Mappers.Repositories.Semantic;

using Microsoft.CodeAnalysis;

using System;

/// <inheritdoc cref="IDetachedMappedSemanticTypeArgumentRecorderFactory{TRecord}"/>
public sealed class DetachedMappedSemanticTypeArgumentRecorderFactory<TRecord> : IDetachedMappedSemanticTypeArgumentRecorderFactory<TRecord>
{
    IDetachedMappedSemanticTypeArgumentRecorder<TRecord> IDetachedMappedSemanticTypeArgumentRecorderFactory<TRecord>.Create(Func<TRecord, ITypeSymbol, bool> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return Create(recorder);
    }

    IDetachedMappedSemanticTypeArgumentRecorder<TRecord> IDetachedMappedSemanticTypeArgumentRecorderFactory<TRecord>.Create(Action<TRecord, ITypeSymbol> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return Create(recorder.TrueReturning());
    }

    private IDetachedMappedSemanticTypeArgumentRecorder<TRecord> Create(Func<TRecord, ITypeSymbol, bool> recorder) => new ArgumentRecorder(recorder);

    private sealed class ArgumentRecorder : IDetachedMappedSemanticTypeArgumentRecorder<TRecord>
    {
        private readonly Func<TRecord, ITypeSymbol, bool> Recorder;

        public ArgumentRecorder(Func<TRecord, ITypeSymbol, bool> recorder)
        {
            Recorder = recorder;
        }

        bool IDetachedMappedSemanticTypeArgumentRecorder<TRecord>.TryRecordArgument(TRecord dataRecord, ITypeSymbol argument)
        {
            if (dataRecord is null)
            {
                throw new ArgumentNullException(nameof(dataRecord));
            }

            if (argument is null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            return Recorder(dataRecord, argument);
        }
    }
}
