namespace SharpAttributeParser.Mappers.MappedRecorders;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;

/// <summary>Records syntactic information about the arguments of some type parameter.</summary>
public interface IMappedSyntacticTypeArgumentRecorder
{
    /// <summary>Attempts to record syntactic information about an argument of some type parameter.</summary>
    /// <param name="syntax">The syntactic description of the argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether syntactic information was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    public abstract bool TryRecordArgument(ExpressionSyntax syntax);
}
