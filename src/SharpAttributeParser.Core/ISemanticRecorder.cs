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
