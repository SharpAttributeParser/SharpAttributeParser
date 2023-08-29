namespace SharpAttributeParser.Mappers.MappedRecorders;

/// <summary>Records the arguments of some constructor parameter.</summary>
public interface IMappedSemanticConstructorArgumentRecorder
{
    /// <summary>Attempts to record an argument of some constructor parameter.</summary>
    /// <param name="argument">The argument of the constructor parameter.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    public abstract bool TryRecordArgument(object? argument);
}
