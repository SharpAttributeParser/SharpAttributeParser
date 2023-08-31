namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Collections.Generic;

/// <summary>Records syntactic information about the arguments of constructor parameters.</summary>
public interface ISyntacticConstructorArgumentRecorder
{
    /// <summary>Attempts to record syntactic information about an argument of a constructor parameter.</summary>
    /// <param name="parameter">The constructor parameter.</param>
    /// <param name="syntax">The syntactic description of the argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether syntactic information was successfully recorded.</returns>
    public abstract bool TryRecordArgument(IParameterSymbol parameter, ExpressionSyntax syntax);

    /// <summary>Attempts to record syntactic information about a <see langword="params"/>-argument of a constructor parameter.</summary>
    /// <param name="parameter">The constructor parameter.</param>
    /// <param name="elementSyntax">The syntactic description of each element in the <see langword="params"/>-argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether syntactic information was successfully recorded.</returns>
    public abstract bool TryRecordParamsArgument(IParameterSymbol parameter, IReadOnlyList<ExpressionSyntax> elementSyntax);

    /// <summary>Attempts to record syntactic information about an unspecified argument of an optional constructor parameter.</summary>
    /// <param name="parameter">The constructor parameter.</param>
    /// <returns>A <see cref="bool"/> indicating whether syntactic information was successfully recorded.</returns>
    public abstract bool TryRecordDefaultArgument(IParameterSymbol parameter);
}
