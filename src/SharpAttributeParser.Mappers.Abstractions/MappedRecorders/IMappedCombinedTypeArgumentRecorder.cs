namespace SharpAttributeParser.Mappers.MappedRecorders;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>Records the arguments of some type parameter, together with syntactic information about the argument.</summary>
public interface IMappedCombinedTypeArgumentRecorder
{
    /// <summary>Attempts to record an argument of some type parameter.</summary>
    /// <param name="argument">The argument of the type parameter.</param>
    /// <param name="syntax">The syntactic description of the argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    public abstract bool TryRecordArgument(ITypeSymbol argument, ExpressionSyntax syntax);
}
