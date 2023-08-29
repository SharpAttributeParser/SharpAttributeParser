namespace SharpAttributeParser;

using System;

/// <summary>Records the arguments of attributes.</summary>
public interface ISemanticRecorder
{
    /// <summary>The recorder used to record the arguments of type parameters.</summary>
    public abstract ISemanticTypeArgumentRecorder TypeArgument { get; }

    /// <summary>The recorder used to record the arguments of constructor parameters.</summary>
    public abstract ISemanticConstructorArgumentRecorder ConstructorArgument { get; }

    /// <summary>The recorder used to record the arguments of named parameters.</summary>
    public abstract ISemanticNamedArgumentRecorder NamedArgument { get; }
}

/// <summary>Records the arguments of attributes.</summary>
/// <typeparam name="TRecord">The type to which arguments are recorded.</typeparam>
public interface ISemanticRecorder<out TRecord> : ISemanticRecorder
{
    /// <summary>Retrieves a record of the recorded arguments.</summary>
    /// <returns>A record of the recorded arguments.</returns>
    /// <exception cref="InvalidOperationException"/>
    public abstract TRecord GetRecord();
}
