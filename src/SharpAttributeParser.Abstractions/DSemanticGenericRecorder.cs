namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System;

/// <summary>Responsible for recording the semantically parsed argument of a type parameter.</summary>
/// <param name="value">The value of the argument.</param>
/// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
/// <exception cref="ArgumentNullException"/>
public delegate bool DSemanticGenericRecorder(ITypeSymbol value);
