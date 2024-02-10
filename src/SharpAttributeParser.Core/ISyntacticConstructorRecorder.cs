namespace SharpAttributeParser;

/// <summary>Records syntactic information about the arguments of constructor parameters.</summary>
public interface ISyntacticConstructorRecorder
{
    /// <summary>Records syntactical information about the normal arguments of constructor parameters.</summary>
    public abstract ISyntacticNormalConstructorRecorder Normal { get; }

    /// <summary>Records syntactical information about the <see langword="params"/>-arguments of constructor parameters.</summary>
    public abstract ISyntacticParamsConstructorRecorder Params { get; }

    /// <summary>Records syntactical information about the unspecified arguments of optional constructor parameters.</summary>
    public abstract ISyntacticDefaultConstructorRecorder Default { get; }
}
