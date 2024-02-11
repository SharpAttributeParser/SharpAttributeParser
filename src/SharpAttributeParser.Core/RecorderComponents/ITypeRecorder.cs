namespace SharpAttributeParser.RecorderComponents;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>Records the arguments of type parameters, together with syntactic information about the arguments.</summary>
public interface ITypeRecorder
{
    /// <summary>Attempts to record an argument of a type parameter, together with syntactic information about the argument.</summary>
    /// <param name="parameter">The type parameter.</param>
    /// <param name="argument">The argument of the type parameter.</param>
    /// <param name="syntax">The syntactic description of the argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    public abstract bool TryRecordArgument(ITypeParameterSymbol parameter, ITypeSymbol argument, ExpressionSyntax syntax);
}
