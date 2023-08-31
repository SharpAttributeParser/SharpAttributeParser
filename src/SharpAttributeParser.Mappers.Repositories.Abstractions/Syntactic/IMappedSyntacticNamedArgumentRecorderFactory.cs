namespace SharpAttributeParser.Mappers.Repositories.Syntactic;

using SharpAttributeParser.Mappers.MappedRecorders;

/// <summary>Handles creation of <see cref="IMappedSyntacticNamedArgumentRecorder"/> using detached recorders.</summary>
public interface IMappedSyntacticNamedArgumentRecorderFactory
{
    /// <summary>Creates an attached recorder.</summary>
    /// <typeparam name="TRecord">The type to which syntactic information about arguments is recorded.</typeparam>
    /// <param name="dataRecord">The record to which syntactic information about arguments is recorded.</param>
    /// <param name="detachedRecorder">The detached recorder, recording syntactic information about arguments to provided records.</param>
    /// <returns>The created recorder.</returns>
    public abstract IMappedSyntacticNamedArgumentRecorder Create<TRecord>(TRecord dataRecord, IDetachedMappedSyntacticNamedArgumentRecorder<TRecord> detachedRecorder);
}
