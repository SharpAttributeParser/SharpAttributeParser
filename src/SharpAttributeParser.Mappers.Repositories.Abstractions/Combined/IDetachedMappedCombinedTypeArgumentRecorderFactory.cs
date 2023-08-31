namespace SharpAttributeParser.Mappers.Repositories.Combined;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;

/// <summary>Handles creation of <see cref="IDetachedMappedCombinedTypeArgumentRecorder{TRecord}"/>.</summary>
/// <typeparam name="TRecord">The type to which arguments are recorded.</typeparam>
public interface IDetachedMappedCombinedTypeArgumentRecorderFactory<TRecord>
{
    /// <summary>Creates a recorder which invokes the provided recorder.</summary>
    /// <param name="recorder">The recorder responsible for recording arguments of the type parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// <item>The syntactic description of the argument.</item>
    /// </list>
    /// The <see cref="bool"/> returned by the recorder should indicate whether the argument was successfully recorded.
    /// </param>
    /// <returns>The created recorder.</returns>
    public abstract IDetachedMappedCombinedTypeArgumentRecorder<TRecord> Create(Func<TRecord, ITypeSymbol, ExpressionSyntax, bool> recorder);

    /// <summary>Creates a recorder which invokes the provided recorder and returns <see langword="true"/>.</summary>
    /// <param name="recorder">The recorder responsible for recording arguments of the type parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// <item>The syntactic description of the argument.</item>
    /// </list></param>
    /// <returns>The created recorder.</returns>
    public abstract IDetachedMappedCombinedTypeArgumentRecorder<TRecord> Create(Action<TRecord, ITypeSymbol, ExpressionSyntax> recorder);
}
