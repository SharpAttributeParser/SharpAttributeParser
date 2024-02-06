namespace SharpAttributeParser;

/// <summary>Records syntactic information about the arguments of attributes.</summary>
public interface ISyntacticRecorder
{
    /// <summary>Records syntactic information about the arguments of type parameters.</summary>
    public abstract ISyntacticTypeRecorder Type { get; }

    /// <summary>Records syntactic information about the arguments of constructor parameters.</summary>
    public abstract ISyntacticConstructorRecorder Constructor { get; }

    /// <summary>Records syntactic information about the arguments of named parameters.</summary>
    public abstract ISyntacticNamedRecorder Named { get; }
}

/// <summary>Records syntactic information about the arguments of attributes.</summary>
/// <typeparam name="TRecord">The type to which syntactic information is recorded.</typeparam>
public interface ISyntacticRecorder<out TRecord> : ISyntacticRecorder
{
    /// <summary>Builds a record of the recorded syntactic information.</summary>
    /// <returns>A record of the recorded syntactic information.</returns>
    public abstract TRecord BuildRecord();
}
