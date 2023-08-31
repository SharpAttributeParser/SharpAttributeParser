namespace SharpAttributeParser.Mappers;

/// <summary>Handles creation of <see cref="ISyntacticRecorder"/> using mappers.</summary>
public interface ISyntacticRecorderFactory
{
    /// <summary>Creates a recorder which records syntactic information about the arguments of attributes to the provided record.</summary>
    /// <typeparam name="TRecord">The type to which syntactic information about arguments is recorded.</typeparam>
    /// <param name="mapper">Maps parameters of the attribute to recorders, responsible for recording syntactic information about arguments of that parameter.</param>
    /// <param name="dataRecord">The record to which syntactic information about arguments is recorded by the created recorder.</param>
    /// <returns>The created recorder.</returns>
    public abstract ISyntacticRecorder<TRecord> Create<TRecord>(ISyntacticMapper<TRecord> mapper, TRecord dataRecord);

    /// <summary>Creates a recorder which records syntactic information about the arguments of attributes through the provided record builder.</summary>
    /// <typeparam name="TRecord">The type representing the recorded syntactic information.</typeparam>
    /// <typeparam name="TRecordBuilder">The type through which syntactic information about arguments is recorded, and which builds records.</typeparam>
    /// <param name="mapper">Maps parameters of the attribute to recorders, responsible for recording syntactic information about arguments of that parameter.</param>
    /// <param name="recordBuilder">The record builder through which syntactic information about arguments is recorded by the created recorder.</param>
    /// <returns>The created recorder.</returns>
    public abstract ISyntacticRecorder<TRecord> Create<TRecord, TRecordBuilder>(ISyntacticMapper<TRecordBuilder> mapper, TRecordBuilder recordBuilder) where TRecordBuilder : IRecordBuilder<TRecord>;
}
