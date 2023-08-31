namespace SharpAttributeParser.Mappers.Repositories.Combined;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Collections.Generic;

/// <summary>Records the arguments of some constructor parameter, together with syntactic information about the arguments, to provided records.</summary>
/// <typeparam name="TRecord">The type to which arguments are recorded.</typeparam>
public interface IDetachedMappedCombinedConstructorArgumentRecorder<in TRecord>
{
    /// <summary>Attempts to record an argument of some constructor parameter, together with syntactic information about the argument.</summary>
    /// <param name="dataRecord">The record to which the argument is recorded.</param>
    /// <param name="argument">The argument of the constructor parameter.</param>
    /// <param name="syntax">The syntactic description of the argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    public abstract bool TryRecordArgument(TRecord dataRecord, object? argument, ExpressionSyntax syntax);

    /// <summary>Attempts to record a <see langword="params"/>-argument of some constructor parameter, together with syntactic information about the argument.</summary>
    /// <param name="dataRecord">The record to which the argument is recorded.</param>
    /// <param name="argument">The argument of the constructor parameter.</param>
    /// <param name="elementSyntax">The syntactic description of each element in the <see langword="params"/>-argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    public abstract bool TryRecordParamsArgument(TRecord dataRecord, object? argument, IReadOnlyList<ExpressionSyntax> elementSyntax);

    /// <summary>Attempts to record an unspecified argument of some optional constructor parameter, together with syntactic information about the argument.</summary>
    /// <param name="dataRecord">The record to which the argument is recorded.</param>
    /// <param name="argument">The argument of the constructor parameter.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    public abstract bool TryRecordDefaultArgument(TRecord dataRecord, object? argument);
}
