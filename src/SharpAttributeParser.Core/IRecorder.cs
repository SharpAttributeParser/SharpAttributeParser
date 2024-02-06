namespace SharpAttributeParser;

/// <summary>Records the arguments of attributes, together with syntactic information about the arguments.</summary>
public interface IRecorder
{
    /// <summary>Records the arguments of type parameters.</summary>
    public abstract ITypeRecorder Type { get; }

    /// <summary>Records the arguments of constructor parameters.</summary>
    public abstract IConstructorRecorder Constructor { get; }

    /// <summary>Records the arguments of named parameters.</summary>
    public abstract INamedRecorder Named { get; }
}

/// <summary>Records the arguments of attributes, together with syntactic information about the arguments.</summary>
/// <typeparam name="TRecord">The type to which arguments are recorded.</typeparam>
public interface IRecorder<out TRecord> : IRecorder
{
    /// <summary>Builds a record of the recorded arguments.</summary>
    /// <returns>A record of the recorded arguments.</returns>
    public abstract TRecord BuildRecord();
}
