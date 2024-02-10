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
