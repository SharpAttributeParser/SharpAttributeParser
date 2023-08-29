namespace SharpAttributeParser.Mappers.MappedRecorders;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;

/// <summary>Records the arguments of some named parameter.</summary>
public interface IMappedCombinedNamedArgumentRecorder
{
    /// <summary>Attempts to record an argument of some named parameter.</summary>
    /// <param name="argument">The argument of the named parameter.</param>
    /// <param name="syntax">The syntactic description of the argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    public abstract bool TryRecordArgument(object? argument, ExpressionSyntax syntax);
}
