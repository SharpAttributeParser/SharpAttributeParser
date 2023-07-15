namespace SharpAttributeParser;

/// <summary>Records the argument of some attribute parameter.</summary>
public interface ISemanticAttributeArgumentRecorder
{
    /// <summary>Records the provided attribute argument.</summary>
    /// <param name="argument">The recorded argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    public abstract bool RecordArgument(object? argument);
}
