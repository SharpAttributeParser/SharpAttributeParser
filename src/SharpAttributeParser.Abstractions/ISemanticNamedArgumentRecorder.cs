namespace SharpAttributeParser;

using System;

/// <summary>Records the arguments of named parameters.</summary>
public interface ISemanticNamedArgumentRecorder
{
    /// <summary>Attempts to record an argument of a named parameter.</summary>
    /// <param name="parameterName">The name of the named parameter.</param>
    /// <param name="argument">The argument of the named parameter.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract bool TryRecordArgument(string parameterName, object? argument);
}
