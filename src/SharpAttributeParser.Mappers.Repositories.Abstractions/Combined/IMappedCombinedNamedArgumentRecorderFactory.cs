namespace SharpAttributeParser.Mappers.Repositories.Combined;

using SharpAttributeParser.Mappers.MappedRecorders;

/// <summary>Handles creation of <see cref="IMappedCombinedNamedArgumentRecorder"/> using detached recorders.</summary>
public interface IMappedCombinedNamedArgumentRecorderFactory
{
    /// <summary>Creates an attached recorder.</summary>
    /// <typeparam name="TRecord">The type to which arguments are recorded.</typeparam>
    /// <param name="dataRecord">The record to which arguments are recorded.</param>
    /// <param name="detachedRecorder">The detached recorder, recording arguments to provided records.</param>
    /// <returns>The created recorder.</returns>
    public abstract IMappedCombinedNamedArgumentRecorder Create<TRecord>(TRecord dataRecord, IDetachedMappedCombinedNamedArgumentRecorder<TRecord> detachedRecorder);
}
