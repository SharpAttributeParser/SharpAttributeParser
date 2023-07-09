namespace SharpAttributeParser;

using System;

/// <summary>Handles production of <see cref="ISemanticAttributeRecorder"/>.</summary>
public interface ISemanticAttributeRecorderFactory
{
    /// <summary>Produces a <see cref="ISemanticAttributeRecorder"/>, recording attribute arguments to the provided <typeparamref name="TRecord"/>.</summary>
    /// <typeparam name="TRecord">The type to which the produced <see cref="ISemanticAttributeRecorder"/> records attribute arguments.</typeparam>
    /// <param name="argumentMapper">Maps the attribute parameters to <see cref="ISemanticAttributeArgumentRecorder"/>, responsible for recording the arguments of that parameter.</param>
    /// <param name="dataRecord">The <typeparamref name="TRecord"/> to which the produced <see cref="ISemanticAttributeRecorder"/> records attribute arguments.</param>
    /// <returns>The produced <see cref="ISemanticAttributeRecorder"/>.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract ISemanticAttributeRecorder<TRecord> Create<TRecord>(ISemanticAttributeMapper<TRecord> argumentMapper, TRecord dataRecord);

    /// <summary>Produces a <see cref="ISemanticAttributeRecorder"/>, recording attribute arguments through the provided <typeparamref name="TRecordBuild"/>.</summary>
    /// <typeparam name="TRecord">The type representing the recorded attribute arguments.</typeparam>
    /// <typeparam name="TRecordBuild">The type through which the produced <see cref="ISemanticAttributeRecorder"/> records attribute arguments.</typeparam>
    /// <param name="argumentMapper">Maps the attribute parameters to <see cref="ISemanticAttributeArgumentRecorder"/>, responsible for recording the arguments of that parameter.</param>
    /// <param name="recordBuilder">The <typeparamref name="TRecordBuild"/> through which the produced <see cref="ISemanticAttributeRecorder"/> records attribute arguments, and which can build a <typeparamref name="TRecord"/>.</param>
    /// <returns>The produced <see cref="ISemanticAttributeRecorder"/>.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract ISemanticAttributeRecorder<TRecord> Create<TRecord, TRecordBuild>(ISemanticAttributeMapper<TRecordBuild> argumentMapper, TRecordBuild recordBuilder) where TRecordBuild : IRecordBuilder<TRecord>;
}
