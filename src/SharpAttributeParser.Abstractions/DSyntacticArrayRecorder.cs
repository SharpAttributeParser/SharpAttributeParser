namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;

/// <summary>Responsible for recording the syntactically parsed array-valued argument of a constructor or named parameter.</summary>
/// <param name="value">The value of the argument.</param>
/// <param name="collectionLocation">The <see cref="Location"/> of the entire argument.</param>
/// <param name="elementLocations">The <see cref="Location"/> of the individual elements in the array-valued argument.</param>
/// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
/// <exception cref="ArgumentException"/>
/// <exception cref="ArgumentNullException"/>
public delegate bool DSyntacticArrayRecorder(IReadOnlyList<object?>? value, Location collectionLocation, IReadOnlyList<Location> elementLocations);
