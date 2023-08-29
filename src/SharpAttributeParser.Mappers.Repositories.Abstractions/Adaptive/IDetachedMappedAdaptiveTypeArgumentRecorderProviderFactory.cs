namespace SharpAttributeParser.Mappers.Repositories.Adaptive;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;

/// <summary>Handles creation of <see cref="IDetachedMappedAdaptiveTypeArgumentRecorderProvider{TCombinedRecord, TSemanticRecord}"/> related to type parameters.</summary>
/// <typeparam name="TCombinedRecord">The type to which arguments are recorded when parsed with syntactic context.</typeparam>
/// <typeparam name="TSemanticRecord">The type to which arguments are recorded when parsed without syntactic context.</typeparam>
public interface IDetachedMappedAdaptiveTypeArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>
{
    /// <summary>Creates a provider of recorders which invoke the provided recorders.</summary>
    /// <param name="combinedRecorder">The recorder responsible for recording arguments of the type parameter when parsed with syntactic context. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// <item>The syntactic description of the argument.</item>
    /// </list>
    /// The <see cref="bool"/> returned by the recorder should indicate whether the argument was successfully recorded.
    /// </param>
    /// <param name="semanticRecorder">The recorder responsible for recording arguments of the type parameter when parsed without syntactic context. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// </list>
    /// The <see cref="bool"/> returned by the recorder should indicate whether the argument was successfully recorded.
    /// </param>
    /// <returns>The created provider.</returns>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract IDetachedMappedAdaptiveTypeArgumentRecorderProvider<TCombinedRecord, TSemanticRecord> Create(Func<TCombinedRecord, ITypeSymbol, ExpressionSyntax, bool> combinedRecorder, Func<TSemanticRecord, ITypeSymbol, bool> semanticRecorder);

    /// <summary>Creates a provider of recorders which invoke the provided recorder and return <see langword="true"/>.</summary>
    /// <param name="combinedRecorder">The recorder responsible for recording arguments of the type parameter when parsed with syntactic context. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// <item>The syntactic description of the argument.</item>
    /// </list></param>
    /// <param name="semanticRecorder">The recorder responsible for recording arguments of the type parameter when parsed without syntactic context. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// </list></param>
    /// <returns>The created provider.</returns>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract IDetachedMappedAdaptiveTypeArgumentRecorderProvider<TCombinedRecord, TSemanticRecord> Create(Action<TCombinedRecord, ITypeSymbol, ExpressionSyntax> combinedRecorder, Action<TSemanticRecord, ITypeSymbol> semanticRecorder);
}
