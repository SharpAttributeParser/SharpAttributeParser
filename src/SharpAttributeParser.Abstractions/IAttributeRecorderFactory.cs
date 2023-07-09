namespace SharpAttributeParser;

using System;

/// <summary>Handles production of <see cref="IAttributeRecorder"/>.</summary>
public interface IAttributeRecorderFactory
{
    /// <summary>Produces an <see cref="IAttributeRecorder"/>, recording attribute arguments to the provided <typeparamref name="TRecord"/>.</summary>
    /// <typeparam name="TRecord">The type to which the produced <see cref="IAttributeRecorder"/> records attribute arguments.</typeparam>
    /// <param name="argumentMapper">Maps parameters of the attribute to <see cref="IAttributeArgumentRecorder"/>, responsible for recording arguments of the parameter.</param>
    /// <param name="dataRecord">The <typeparamref name="TRecord"/> to which the produced <see cref="IAttributeRecorder"/> records attribute arguments.</param>
    /// <returns>The produced <see cref="IAttributeRecorder"/>.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract IAttributeRecorder<TRecord> Create<TRecord>(IAttributeMapper<TRecord> argumentMapper, TRecord dataRecord);

    /// <summary>Produces a <see cref="IAttributeRecorder"/>, recording attribute arguments using the provided <typeparamref name="TRecordBuild"/>.</summary>
    /// <typeparam name="TRecord">The type representing the recorded attribute arguments, when built by the provided <typeparamref name="TRecordBuild"/>.</typeparam>
    /// <typeparam name="TRecordBuild">The type to which the produced <see cref="IAttributeRecorder"/> records attribute arguments, and which can build a <typeparamref name="TRecord"/>.</typeparam>
    /// <param name="argumentMapper">Maps parameters of the attribute to <see cref="IAttributeArgumentRecorder"/>, responsible for recording arguments of the parameter.</param>
    /// <param name="recordBuilder">The <typeparamref name="TRecordBuild"/> to which the produced <see cref="IAttributeRecorder"/> records attribute arguments, and which can build a <typeparamref name="TRecord"/>.</param>
    /// <returns>The produced <see cref="IAttributeRecorder"/>.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract IAttributeRecorder<TRecord> Create<TRecord, TRecordBuild>(IAttributeMapper<TRecordBuild> argumentMapper, TRecordBuild recordBuilder) where TRecordBuild : IRecordBuilder<TRecord>;
}
