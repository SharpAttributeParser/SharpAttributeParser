namespace SharpAttributeParser.Mappers.Repositories.Split;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;

/// <summary>Handles creation of <see cref="IDetachedMappedSplitTypeArgumentRecorderProvider{TSemanticRecord, TSyntacticRecord}"/> related to type parameters.</summary>
/// <typeparam name="TSemanticRecord">The type to which arguments are recorded.</typeparam>
/// <typeparam name="TSyntacticRecord">The type to which syntactic information about arguments is recorded.</typeparam>
public interface IDetachedMappedSplitTypeArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>
{
    /// <summary>Creates a provider of recorders which invoke the provided recorders.</summary>
    /// <param name="semanticRecorder">The recorder responsible for recording arguments of the type parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// </list>
    /// The <see cref="bool"/> returned by the recorder should indicate whether the argument was successfully recorded.
    /// </param>
    /// <param name="syntacticRecorder">The recorder responsible for recording syntactic information about arguments of the type parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The syntactic description of the argument.</item>
    /// </list>
    /// The <see cref="bool"/> returned by the recorder should indicate whether the argument was successfully recorded.
    /// </param>
    /// <returns>The created provider.</returns>
    public abstract IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> Create(Func<TSemanticRecord, ITypeSymbol, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder);

    /// <summary>Creates a provider of recorders which invoke the provided recorders and return <see langword="true"/>.</summary>
    /// <param name="semanticRecorder">The recorder responsible for recording arguments of the type parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// </list></param>
    /// <param name="syntacticRecorder">The recorder responsible for recording syntactic information about arguments of the type parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The syntactic description of the argument.</item>
    /// </list></param>
    /// <returns>The created provider.</returns>
    public abstract IDetachedMappedSplitTypeArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> Create(Action<TSemanticRecord, ITypeSymbol> semanticRecorder, Action<TSyntacticRecord, ExpressionSyntax> syntacticRecorder);
}
