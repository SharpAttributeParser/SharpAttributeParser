namespace SharpAttributeParser;

/// <summary>Records the argument of some attribute parameter.</summary>
/// <param name="argument">The argument of the attribute parameter.</param>
/// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
public delegate bool DSemanticAttributeArgumentRecorder(object? argument);
