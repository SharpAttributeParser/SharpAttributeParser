namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Collections.Generic;

/// <summary>Records the <see langword="params"/>-arguments of constructor parameters, together with syntactic information about the arguments.</summary>
public interface IParamsConstructorRecorder
{
    /// <summary>Attempts to record a <see langword="params"/>-argument of a constructor parameter, together with syntactic information about the argument.</summary>
    /// <param name="parameter">The constructor parameter.</param>
    /// <param name="argument">The argument of the constructor parameter.</param>
    /// <param name="elementSyntax">The syntactic description of each element in the <see langword="params"/>-argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    public abstract bool TryRecordArgument(IParameterSymbol parameter, object? argument, IReadOnlyList<ExpressionSyntax> elementSyntax);
}
