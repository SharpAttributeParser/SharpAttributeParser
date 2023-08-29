namespace SharpAttributeParser.Mappers.MappedRecorders;

/// <summary>Records the arguments of some named parameter.</summary>
public interface IMappedSemanticNamedArgumentRecorder
{
    /// <summary>Attempts to record an argument of some named parameter.</summary>
    /// <param name="argument">The argument of the named parameter.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    public abstract bool TryRecordArgument(object? argument);
}
