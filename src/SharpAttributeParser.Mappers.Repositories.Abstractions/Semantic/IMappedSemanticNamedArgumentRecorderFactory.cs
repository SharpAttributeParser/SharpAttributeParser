namespace SharpAttributeParser.Mappers.Repositories.Semantic;

using SharpAttributeParser.Mappers.MappedRecorders;

using System;

/// <summary>Handles creation of <see cref="IMappedSemanticNamedArgumentRecorder"/> using detached recorders.</summary>
public interface IMappedSemanticNamedArgumentRecorderFactory
{
    /// <summary>Creates an attached recorder.</summary>
    /// <typeparam name="TRecord">The type to which arguments are recorded.</typeparam>
    /// <param name="dataRecord">The record to which arguments are recorded.</param>
    /// <param name="detachedRecorder">The detached recorder, recording arguments to provided records.</param>
    /// <returns>The created recorder.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract IMappedSemanticNamedArgumentRecorder Create<TRecord>(TRecord dataRecord, IDetachedMappedSemanticNamedArgumentRecorder<TRecord> detachedRecorder);
}
