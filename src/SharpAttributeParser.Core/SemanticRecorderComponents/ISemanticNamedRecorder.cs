namespace SharpAttributeParser.SemanticRecorderComponents;

/// <summary>Records the arguments of named parameters.</summary>
public interface ISemanticNamedRecorder
{
    /// <summary>Attempts to record an argument of a named parameter.</summary>
    /// <param name="parameterName">The name of the named parameter.</param>
    /// <param name="argument">The argument of the named parameter.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    public abstract bool TryRecordArgument(string parameterName, object? argument);
}
