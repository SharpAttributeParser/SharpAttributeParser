namespace SharpAttributeParser.Mappers.Repositories.Combined;

/// <summary>Handles creation of attached recorders using detached recorders.</summary>
public interface IMappedCombinedArgumentRecorderFactory
{
    /// <summary>Handles creation of attached recorders related to type parameters.</summary>
    public abstract IMappedCombinedTypeArgumentRecorderFactory TypeParameter { get; }

    /// <summary>Handles creation of attached recorders related to constructor parameters.</summary>
    public abstract IMappedCombinedConstructorArgumentRecorderFactory ConstructorParameter { get; }

    /// <summary>Handles creation of attached recorders related to named parameters.</summary>
    public abstract IMappedCombinedNamedArgumentRecorderFactory NamedParameter { get; }
}
