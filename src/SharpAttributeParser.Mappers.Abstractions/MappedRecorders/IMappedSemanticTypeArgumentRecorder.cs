﻿namespace SharpAttributeParser.Mappers.MappedRecorders;

using Microsoft.CodeAnalysis;

using System;

/// <summary>Records the arguments of some type parameter.</summary>
public interface IMappedSemanticTypeArgumentRecorder
{
    /// <summary>Attempts to record an argument of some type parameter.</summary>
    /// <param name="argument">The argument of the type parameter.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract bool TryRecordArgument(ITypeSymbol argument);
}
