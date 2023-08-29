namespace SharpAttributeParser.Mappers;

using System;

/// <summary>Handles creation of <see cref="ICombinedRecorder"/> using mappers.</summary>
public interface ICombinedRecorderFactory
{
    /// <summary>Creates a recorder which records the arguments of attributes to the provided record.</summary>
    /// <typeparam name="TRecord">The type to which arguments are recorded.</typeparam>
    /// <param name="mapper">Maps parameters of the attribute to recorders, responsible for recording arguments of that parameter.</param>
    /// <param name="dataRecord">The record to which arguments are recorded by the created recorder.</param>
    /// <returns>The created recorder.</returns>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract ICombinedRecorder<TRecord> Create<TRecord>(ICombinedMapper<TRecord> mapper, TRecord dataRecord);

    /// <summary>Creates a recorder which records the arguments of attributes through the provided record builder.</summary>
    /// <typeparam name="TRecord">The type representing the recorded arguments.</typeparam>
    /// <typeparam name="TRecordBuilder">The type through which arguments are recorded, and which builds records.</typeparam>
    /// <param name="mapper">Maps parameters of the attribute to recorders, responsible for recording arguments of that parameter.</param>
    /// <param name="recordBuilder">The record builder through which arguments are recorded by the created recorder.</param>
    /// <returns>The created recorder.</returns>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract ICombinedRecorder<TRecord> Create<TRecord, TRecordBuilder>(ICombinedMapper<TRecordBuilder> mapper, TRecordBuilder recordBuilder) where TRecordBuilder : IRecordBuilder<TRecord>;
}
