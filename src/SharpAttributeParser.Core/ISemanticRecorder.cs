namespace SharpAttributeParser;

/// <summary>Records the arguments of attributes.</summary>
public interface ISemanticRecorder
{
    /// <summary>Records the arguments of type parameters.</summary>
    public abstract ISemanticTypeRecorder Type { get; }

    /// <summary>Records the arguments of constructor parameters.</summary>
    public abstract ISemanticConstructorRecorder Constructor { get; }

    /// <summary>Records the arguments of named parameters.</summary>
    public abstract ISemanticNamedRecorder Named { get; }
}

/// <summary>Records the arguments of attributes.</summary>
/// <typeparam name="TRecord">The type to which arguments are recorded.</typeparam>
public interface ISemanticRecorder<out TRecord> : ISemanticRecorder
{
    /// <summary>Retrieves a record of the recorded arguments.</summary>
    /// <returns>A record of the recorded arguments.</returns>
    public abstract TRecord GetRecord();
}
