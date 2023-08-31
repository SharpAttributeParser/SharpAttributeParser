namespace SharpAttributeParser;

/// <summary>Records syntactic information about the arguments of attributes.</summary>
public interface ISyntacticRecorder
{
    /// <summary>The recorder used to record syntactic information about the arguments of type parameters.</summary>
    public abstract ISyntacticTypeArgumentRecorder TypeArgument { get; }

    /// <summary>The recorder used to record syntactic information about the arguments of constructor parameters.</summary>
    public abstract ISyntacticConstructorArgumentRecorder ConstructorArgument { get; }

    /// <summary>The recorder used to record syntactic information about the arguments of named parameters.</summary>
    public abstract ISyntacticNamedArgumentRecorder NamedArgument { get; }
}

/// <summary>Records syntactic information about the arguments of attributes.</summary>
/// <typeparam name="TRecord">The type to which syntactic information is recorded.</typeparam>
public interface ISyntacticRecorder<out TRecord> : ISyntacticRecorder
{
    /// <summary>Retrieves a record of the recorded syntactic information.</summary>
    /// <returns>A record of the recorded syntactic information.</returns>
    public abstract TRecord GetRecord();
}
