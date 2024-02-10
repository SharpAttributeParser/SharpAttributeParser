namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>Records syntactic information about the normal arguments of constructor parameters.</summary>
public interface ISyntacticNormalConstructorRecorder
{
    /// <summary>Attempts to record syntactic information about an argument of a constructor parameter.</summary>
    /// <param name="parameter">The constructor parameter.</param>
    /// <param name="syntax">The syntactic description of the argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether syntactic information was successfully recorded.</returns>
    public abstract bool TryRecordArgument(IParameterSymbol parameter, ExpressionSyntax syntax);
}
