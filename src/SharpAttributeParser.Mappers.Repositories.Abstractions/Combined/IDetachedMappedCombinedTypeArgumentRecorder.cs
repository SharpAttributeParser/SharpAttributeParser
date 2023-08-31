namespace SharpAttributeParser.Mappers.Repositories.Combined;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>Records the arguments of some type parameter, together with syntactic information about the arguments, to provided records.</summary>
/// <typeparam name="TRecord">The type to which arguments are recorded.</typeparam>
public interface IDetachedMappedCombinedTypeArgumentRecorder<in TRecord>
{
    /// <summary>Attempts to record an argument of some type parameter, together with syntactic information about the argument.</summary>
    /// <param name="dataRecord">The record to which the argument is recorded.</param>
    /// <param name="argument">The argument of the type parameter.</param>
    /// <param name="syntax">The syntactic description of the argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    public abstract bool TryRecordArgument(TRecord dataRecord, ITypeSymbol argument, ExpressionSyntax syntax);
}
