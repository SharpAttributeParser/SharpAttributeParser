namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;

/// <summary>Records syntactic information about the arguments of type parameters.</summary>
public interface ISyntacticTypeArgumentRecorder
{
    /// <summary>Attempts to record syntactic information about an argument of a type parameter.</summary>
    /// <param name="parameter">The type parameter.</param>
    /// <param name="syntax">The syntactic description of the argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether syntactic information was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract bool TryRecordArgument(ITypeParameterSymbol parameter, ExpressionSyntax syntax);
}
