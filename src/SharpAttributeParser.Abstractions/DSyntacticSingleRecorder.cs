namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System;

/// <summary>Responsible for recording the syntactically parsed argument of a constructor or named parameter.</summary>
/// <param name="value">The value of the argument.</param>
/// <param name="location">The <see cref="Location"/> of the argument.</param>
/// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
/// <exception cref="ArgumentNullException"/>
public delegate bool DSyntacticSingleRecorder(object? value, Location location);
