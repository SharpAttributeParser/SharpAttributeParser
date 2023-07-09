namespace SharpAttributeParser;

using System;

/// <summary>Handles production of <see cref="ISyntacticAttributeRecorder"/> using <see cref="ISyntacticAttributeMapper{TRecord}"/>.</summary>
public interface ISyntacticAttributeRecorderFactory
{
    /// <summary>Produces a <see cref="ISyntacticAttributeRecorder"/>, recording syntactical information about attribute arguments to the provided <typeparamref name="TRecord"/>.</summary>
    /// <typeparam name="TRecord">The type to which the produced <see cref="ISyntacticAttributeRecorder"/> records syntactical information.</typeparam>
    /// <param name="argumentMapper">Maps the attribute parameters to <see cref="ISyntacticAttributeArgumentRecorder"/>, responsible for recording syntactical information about arguments of that parameter.</param>
    /// <param name="dataRecord">The <typeparamref name="TRecord"/> to which the produced <see cref="ISyntacticAttributeRecorder"/> records syntactical information.</param>
    /// <returns>The produced <see cref="ISyntacticAttributeRecorder"/>.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract ISyntacticAttributeRecorder<TRecord> Create<TRecord>(ISyntacticAttributeMapper<TRecord> argumentMapper, TRecord dataRecord);

    /// <summary>Produces a <see cref="ISyntacticAttributeRecorder"/>, recording syntactical information about attribute arguments using the provided <typeparamref name="TRecordBuilder"/>.</summary>
    /// <typeparam name="TRecord">The type representing the recorded syntactical information, when built by the provided <typeparamref name="TRecordBuilder"/>.</typeparam>
    /// <typeparam name="TRecordBuilder">The type to which the produced <see cref="ISyntacticAttributeRecorder"/> records syntactical information, and which can build a <typeparamref name="TRecord"/>.</typeparam>
    /// <param name="argumentMapper">Maps the attribute parameters to <see cref="ISyntacticAttributeArgumentRecorder"/>, responsible for recording syntactical information about arguments of that parameter.</param>
    /// <param name="recordBuilder">The <typeparamref name="TRecordBuilder"/> to which the produced <see cref="ISyntacticAttributeRecorder"/> records syntactical information, and which can build a <typeparamref name="TRecord"/>.</param>
    /// <returns>The produced <see cref="ISyntacticAttributeRecorder"/>.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract ISyntacticAttributeRecorder<TRecord> Create<TRecord, TRecordBuilder>(ISyntacticAttributeMapper<TRecordBuilder> argumentMapper, TRecordBuilder recordBuilder) where TRecordBuilder : IRecordBuilder<TRecord>;
}
