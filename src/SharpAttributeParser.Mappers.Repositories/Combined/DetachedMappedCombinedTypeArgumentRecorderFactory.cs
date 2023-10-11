namespace SharpAttributeParser.Mappers.Repositories.Combined;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;

/// <inheritdoc cref="IDetachedMappedCombinedTypeArgumentRecorderFactory{TRecord}"/>
public sealed class DetachedMappedCombinedTypeArgumentRecorderFactory<TRecord> : IDetachedMappedCombinedTypeArgumentRecorderFactory<TRecord>
{
    IDetachedMappedCombinedTypeArgumentRecorder<TRecord> IDetachedMappedCombinedTypeArgumentRecorderFactory<TRecord>.Create(Func<TRecord, ITypeSymbol, ExpressionSyntax, bool> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return Create(recorder);
    }

    IDetachedMappedCombinedTypeArgumentRecorder<TRecord> IDetachedMappedCombinedTypeArgumentRecorderFactory<TRecord>.Create(Action<TRecord, ITypeSymbol, ExpressionSyntax> recorder)
    {
        if (recorder is null)
        {
            throw new ArgumentNullException(nameof(recorder));
        }

        return Create(recorder.TrueReturning());
    }

    private IDetachedMappedCombinedTypeArgumentRecorder<TRecord> Create(Func<TRecord, ITypeSymbol, ExpressionSyntax, bool> recorder) => new ArgumentRecorder(recorder);

    private sealed class ArgumentRecorder : IDetachedMappedCombinedTypeArgumentRecorder<TRecord>
    {
        private readonly Func<TRecord, ITypeSymbol, ExpressionSyntax, bool> Recorder;

        public ArgumentRecorder(Func<TRecord, ITypeSymbol, ExpressionSyntax, bool> recorder)
        {
            Recorder = recorder;
        }

        bool IDetachedMappedCombinedTypeArgumentRecorder<TRecord>.TryRecordArgument(TRecord dataRecord, ITypeSymbol argument, ExpressionSyntax syntax)
        {
            if (dataRecord is null)
            {
                throw new ArgumentNullException(nameof(dataRecord));
            }

            if (argument is null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            if (syntax is null)
            {
                throw new ArgumentNullException(nameof(syntax));
            }

            return Recorder(dataRecord, argument, syntax);
        }
    }
}
