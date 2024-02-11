namespace SharpAttributeParser;

using SharpAttributeParser.SyntacticRecorderComponents;

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
