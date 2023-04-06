namespace SharpAttributeParser;

using System.Collections.Generic;

/// <summary>Responsible for recording the semantically parsed array-valued argument of a constructor or named parameter.</summary>
/// <param name="value">The value of the argument.</param>
/// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
public delegate bool DSemanticArrayRecorder(IReadOnlyList<object?>? value);
