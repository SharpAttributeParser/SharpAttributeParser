namespace SharpAttributeParser.Mappers.Repositories.Semantic;

using Microsoft.CodeAnalysis;

using System;

/// <summary>Handles creation of <see cref="IDetachedMappedSemanticTypeArgumentRecorder{TRecord}"/>.</summary>
/// <typeparam name="TRecord">The type to which arguments are recorded.</typeparam>
public interface IDetachedMappedSemanticTypeArgumentRecorderFactory<TRecord>
{
    /// <summary>Creates a recorder which invokes the provided recorder.</summary>
    /// <param name="recorder">The recorder responsible for recording arguments of the type parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// </list>
    /// The <see cref="bool"/> returned by the recorder should indicate whether the argument was successfully recorded.
    /// </param>
    /// <returns>The created recorder.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract IDetachedMappedSemanticTypeArgumentRecorder<TRecord> Create(Func<TRecord, ITypeSymbol, bool> recorder);

    /// <summary>Creates a recorder which invokes the provided recorder and returns <see langword="true"/>.</summary>
    /// <param name="recorder">The recorder responsible for recording arguments of the type parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// </list></param>
    /// <returns>The created recorder.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract IDetachedMappedSemanticTypeArgumentRecorder<TRecord> Create(Action<TRecord, ITypeSymbol> recorder);
}
