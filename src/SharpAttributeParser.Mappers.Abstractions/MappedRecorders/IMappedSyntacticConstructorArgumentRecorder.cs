namespace SharpAttributeParser.Mappers.MappedRecorders;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;

/// <summary>Records syntactic information about the arguments of some constructor parameter.</summary>
public interface IMappedSyntacticConstructorArgumentRecorder
{
    /// <summary>Attempts to record syntactic information about an argument of some constructor parameter.</summary>
    /// <param name="syntax">The syntactic description of the argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether syntactic information was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    public abstract bool TryRecordArgument(ExpressionSyntax syntax);

    /// <summary>Attempts to record syntactic information about a <see langword="params"/>-argument of a constructor parameter.</summary>
    /// <param name="elementSyntax">The syntactic description of each element in the <see langword="params"/>-argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether syntactic information was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    public abstract bool TryRecordParamsArgument(IReadOnlyList<ExpressionSyntax> elementSyntax);

    /// <summary>Attempts to record syntactic information about an unspecified argument of an optional constructor parameter.</summary>
    /// <returns>A <see cref="bool"/> indicating whether syntactic information was successfully recorded.</returns>
    public abstract bool TryRecordDefaultArgument();
}
