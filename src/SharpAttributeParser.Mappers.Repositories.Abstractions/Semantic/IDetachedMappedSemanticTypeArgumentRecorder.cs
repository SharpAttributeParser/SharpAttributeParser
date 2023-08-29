namespace SharpAttributeParser.Mappers.Repositories.Semantic;

using Microsoft.CodeAnalysis;

using System;

/// <summary>Records the arguments of some type parameter to provided records.</summary>
/// <typeparam name="TRecord">The type to which arguments are recorded.</typeparam>
public interface IDetachedMappedSemanticTypeArgumentRecorder<in TRecord>
{
    /// <summary>Attempts to record an argument of some type parameter.</summary>
    /// <param name="dataRecord">The record to which the argument is recorded.</param>
    /// <param name="argument">The argument of the type parameter.</param>
    /// <returns>A <see cref="bool"/> indicating whether the argument was successfully recorded.</returns>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract bool TryRecordArgument(TRecord dataRecord, ITypeSymbol argument);
}
