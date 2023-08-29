namespace SharpAttributeParser.Mappers.Repositories.Syntactic;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;

/// <summary>Records syntactic information about the arguments of some constructor parameter to provided records.</summary>
/// <typeparam name="TRecord">The type to which syntactic information is recorded.</typeparam>
public interface IDetachedMappedSyntacticConstructorArgumentRecorder<in TRecord>
{
    /// <summary>Attempts to record syntactic information about an argument of a constructor parameter.</summary>
    /// <param name="dataRecord">The record to which syntactic information is recorded.</param>
    /// <param name="syntax">The syntactic description of the argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether syntactic information was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract bool TryRecordArgument(TRecord dataRecord, ExpressionSyntax syntax);

    /// <summary>Attempts to record syntactic information about a <see langword="params"/>-argument of a constructor parameter.</summary>
    /// <param name="dataRecord">The record to which syntactic information is recorded.</param>
    /// <param name="elementSyntax">The syntactic description of each element in the <see langword="params"/>-argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether syntactic information was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract bool TryRecordParamsArgument(TRecord dataRecord, IReadOnlyList<ExpressionSyntax> elementSyntax);

    /// <summary>Attempts to record syntactic information about an unspecified argument of an optional constructor parameter.</summary>
    /// <param name="dataRecord">The record to which syntactic information is recorded.</param>
    /// <returns>A <see cref="bool"/> indicating whether syntactic information was successfully recorded.</returns>
    /// <exception cref="InvalidOperationException"/>
    public abstract bool TryRecordDefaultArgument(TRecord dataRecord);
}
