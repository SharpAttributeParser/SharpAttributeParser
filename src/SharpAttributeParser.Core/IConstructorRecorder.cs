namespace SharpAttributeParser;

/// <summary>Records the arguments of constructor parameters, together with syntactic information about the arguments.</summary>
public interface IConstructorRecorder
{
    /// <summary>Records the normal arguments of constructor parameters.</summary>
    public abstract INormalConstructorRecorder Normal { get; }

    /// <summary>Records the <see langword="params"/>-arguments of constructor parameters.</summary>
    public abstract IParamsConstructorRecorder Params { get; }

    /// <summary>Records the unspecified arguments of optional constructor parameters.</summary>
    public abstract IDefaultConstructorRecorder Default { get; }
}
