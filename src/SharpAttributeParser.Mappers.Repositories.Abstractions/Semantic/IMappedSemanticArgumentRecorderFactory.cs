namespace SharpAttributeParser.Mappers.Repositories.Semantic;

/// <summary>Handles creation of attached recorders using detached recorders.</summary>
public interface IMappedSemanticArgumentRecorderFactory
{
    /// <summary>Handles creation of attached recorders related to type parameters.</summary>
    public abstract IMappedSemanticTypeArgumentRecorderFactory TypeParameter { get; }

    /// <summary>Handles creation of attached recorders related to constructor parameters.</summary>
    public abstract IMappedSemanticConstructorArgumentRecorderFactory ConstructorParameter { get; }

    /// <summary>Handles creation of attached recorders related to named parameters.</summary>
    public abstract IMappedSemanticNamedArgumentRecorderFactory NamedParameter { get; }
}
