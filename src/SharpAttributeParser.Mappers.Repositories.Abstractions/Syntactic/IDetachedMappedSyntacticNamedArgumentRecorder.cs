namespace SharpAttributeParser.Mappers.Repositories.Syntactic;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;

/// <summary>Records syntactic information about the arguments of some named parameter to provided records.</summary>
/// <typeparam name="TRecord">The type to which syntactic information about arguments is recorded.</typeparam>
public interface IDetachedMappedSyntacticNamedArgumentRecorder<in TRecord>
{
    /// <summary>Attempts to record syntactic information about an argument of some named parameter.</summary>
    /// <param name="dataRecord">The record to which syntactic information is recorded.</param>
    /// <param name="syntax">The syntactic description of the argument.</param>
    /// <returns>A <see cref="bool"/> indicating whether syntactic information was successfully recorded.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract bool TryRecordArgument(TRecord dataRecord, ExpressionSyntax syntax);
}
