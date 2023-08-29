namespace SharpAttributeParser;

using System;

/// <summary>Records the arguments of attributes, together with syntactic information about the arguments.</summary>
public interface ICombinedRecorder
{
    /// <summary>The recorder used to record the arguments of type parameters.</summary>
    public abstract ICombinedTypeArgumentRecorder TypeArgument { get; }

    /// <summary>The recorder used to record the arguments of constructor parameters.</summary>
    public abstract ICombinedConstructorArgumentRecorder ConstructorArgument { get; }

    /// <summary>The recorder used to record the arguments of named parameters.</summary>
    public abstract ICombinedNamedArgumentRecorder NamedArgument { get; }
}

/// <summary>Records the arguments of attributes, together with syntactic information about the arguments.</summary>
/// <typeparam name="TRecord">The type to which arguments are recorded.</typeparam>
public interface ICombinedRecorder<out TRecord> : ICombinedRecorder
{
    /// <summary>Retrieves a record of the recorded arguments.</summary>
    /// <returns>A record of the recorded arguments.</returns>
    /// <exception cref="InvalidOperationException"/>
    public abstract TRecord GetRecord();
}
