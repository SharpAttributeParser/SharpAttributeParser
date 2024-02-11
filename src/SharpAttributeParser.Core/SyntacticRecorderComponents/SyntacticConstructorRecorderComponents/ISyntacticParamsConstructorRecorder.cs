namespace SharpAttributeParser.SyntacticRecorderComponents.SyntacticConstructorRecorderComponents;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Collections.Generic;

/// <summary>Records syntactic information about the <see langword="params"/>-arguments of constructor parameters.</summary>
public interface ISyntacticParamsConstructorRecorder
{
    /// <summary>Attempts to record syntactic information about a <see langword="params"/>-argument of a constructor parameter.</summary>
    /// <param name="parameter">The constructor parameter.</param>
    /// <param name="elementSyntax">The syntactic description of each element in the <see langword="params"/>-argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    public abstract bool TryRecordArgument(IParameterSymbol parameter, IReadOnlyList<ExpressionSyntax> elementSyntax);
}
