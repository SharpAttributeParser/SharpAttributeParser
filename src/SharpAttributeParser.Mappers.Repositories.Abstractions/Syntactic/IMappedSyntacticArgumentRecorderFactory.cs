namespace SharpAttributeParser.Mappers.Repositories.Syntactic;

/// <summary>Handles creation of attached recorders using detached recorders.</summary>
public interface IMappedSyntacticArgumentRecorderFactory
{
    /// <summary>Handles creation of attached recorders related to type parameters.</summary>
    public abstract IMappedSyntacticTypeArgumentRecorderFactory TypeParameter { get; }

    /// <summary>Handles creation of attached recorders related to constructor parameters.</summary>
    public abstract IMappedSyntacticConstructorArgumentRecorderFactory ConstructorParameter { get; }

    /// <summary>Handles creation of attached recorders related to named parameters.</summary>
    public abstract IMappedSyntacticNamedArgumentRecorderFactory NamedParameter { get; }
}
