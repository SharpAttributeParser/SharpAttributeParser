namespace SharpAttributeParser.Mappers.MappedRecorders;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;

/// <summary>Records the arguments of some constructor parameter.</summary>
public interface IMappedCombinedConstructorArgumentRecorder
{
    /// <summary>Attempts to record an argument of some constructor parameter.</summary>
    /// <param name="argument">The argument of the constructor parameter.</param>
    /// <param name="syntax">The syntactic description of the argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    public abstract bool TryRecordArgument(object? argument, ExpressionSyntax syntax);

    /// <summary>Attempts to record a <see langword="params"/>-argument of some constructor parameter.</summary>
    /// <param name="argument">The argument of the constructor parameter.</param>
    /// <param name="elementSyntax">The syntactic description of each element in the <see langword="params"/>-argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    public abstract bool TryRecordParamsArgument(object? argument, IReadOnlyList<ExpressionSyntax> elementSyntax);

    /// <summary>Attempts to record an unspecified argument of some optional constructor parameter.</summary>
    /// <param name="argument">The argument of the constructor parameter.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    public abstract bool TryRecordDefaultArgument(object? argument);
}
