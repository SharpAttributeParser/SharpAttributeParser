namespace SharpAttributeParser.Mappers.Repositories.Split;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser.Patterns;

using System;

/// <summary>Handles creation of <see cref="IDetachedMappedSplitConstructorArgumentRecorderProvider{TSemanticRecord, TSyntacticRecord}"/> related to non-optional, non-<see langword="params"/> constructor parameters.</summary>
/// <typeparam name="TSemanticRecord">The type to which arguments are recorded.</typeparam>
/// <typeparam name="TSyntacticRecord">The type to which syntactic information about arguments is recorded.</typeparam>
public interface IDetachedMappedSplitNormalConstructorArgumentRecorderProviderFactory<TSemanticRecord, TSyntacticRecord>
{
    /// <summary>Creates a provider of recorders which invoke the provided recorders.</summary>
    /// <param name="semanticRecorder">The recorder responsible for recording arguments of the constructor parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// </list>
    /// The <see cref="bool"/> returned by the recorder should indicate whether the argument was successfully recorded.
    /// </param>
    /// <param name="syntacticRecorder">The recorder responsible for recording syntactic information about arguments of the constructor parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The syntactic description of the argument.</item>
    /// </list>
    /// The <see cref="bool"/> returned by the recorder should indicate whether the argument was successfully recorded.
    /// </param>
    /// <returns>The created provider.</returns>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> Create(Func<TSemanticRecord, object?, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder);

    /// <summary>Creates a provider of recorders which invoke the provided recorders and return <see langword="true"/>.</summary>
    /// <param name="semanticRecorder">The recorder responsible for recording arguments of the constructor parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// </list></param>
    /// <param name="syntacticRecorder">The recorder responsible for recording syntactic information about arguments of the constructor parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The syntactic description of the argument.</item>
    /// </list></param>
    /// <returns>The created provider.</returns>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> Create(Action<TSemanticRecord, object?> semanticRecorder, Action<TSyntacticRecord, ExpressionSyntax> syntacticRecorder);

    /// <summary>Creates a provider of recorders which filter arguments using the provided pattern before invoking the provided recorders, and which return <see langword="false"/> for discarded arguments.</summary>
    /// <typeparam name="TArgument">The type of the recorded arguments.</typeparam>
    /// <param name="pattern">The pattern used to filter arguments before invoking recorders.</param>
    /// <param name="semanticRecorder">The recorder responsible for recording arguments of the constructor parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// </list>
    /// The <see cref="bool"/> returned by the recorder should indicate whether the argument was successfully recorded.
    /// </param>
    /// <param name="syntacticRecorder">The recorder responsible for recording syntactic information about arguments of the constructor parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The syntactic description of the argument.</item>
    /// </list>
    /// The <see cref="bool"/> returned by the recorder should indicate whether the argument was successfully recorded.
    /// </param>
    /// <returns>The created provider.</returns>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> Create<TArgument>(IArgumentPattern<TArgument> pattern, Func<TSemanticRecord, TArgument, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder);

    /// <summary>Creates a provider of recorders which filter arguments using the provided pattern before invoking the provided recorders and returning <see langword="true"/>, and which return <see langword="false"/> for discarded arguments.</summary>
    /// <typeparam name="TArgument">The type of the recorded arguments.</typeparam>
    /// <param name="pattern">The pattern used to filter arguments before invoking recorders.</param>
    /// <param name="semanticRecorder">The recorder responsible for recording arguments of the constructor parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// </list></param>
    /// <param name="syntacticRecorder">The recorder responsible for recording syntactic information about arguments of the constructor parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The syntactic description of the argument.</item>
    /// </list></param>
    /// <returns>The created provider.</returns>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> Create<TArgument>(IArgumentPattern<TArgument> pattern, Action<TSemanticRecord, TArgument> semanticRecorder, Action<TSyntacticRecord, ExpressionSyntax> syntacticRecorder);

    /// <summary>Creates a provider of recorders which filter arguments using the provided pattern before invoking the provided recorders, and which return <see langword="false"/> for discarded arguments.</summary>
    /// <typeparam name="TArgument">The type of the recorded arguments.</typeparam>
    /// <param name="patternDelegate">Creates the pattern used to filter arguments before invoking recorders.</param>
    /// <param name="semanticRecorder">The recorder responsible for recording arguments of the constructor parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// </list>
    /// The <see cref="bool"/> returned by the recorder should indicate whether the argument was successfully recorded.
    /// </param>
    /// <param name="syntacticRecorder">The recorder responsible for recording syntactic information about arguments of the constructor parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The syntactic description of the argument.</item>
    /// </list>
    /// The <see cref="bool"/> returned by the recorder should indicate whether the argument was successfully recorded.
    /// </param>
    /// <returns>The created provider.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> Create<TArgument>(Func<IArgumentPatternFactory, IArgumentPattern<TArgument>> patternDelegate, Func<TSemanticRecord, TArgument, bool> semanticRecorder, Func<TSyntacticRecord, ExpressionSyntax, bool> syntacticRecorder);

    /// <summary>Creates a provider of recorders which filter arguments using the provided pattern before invoking the provided recorders and returning <see langword="true"/>, and which return <see langword="false"/> for discarded arguments.</summary>
    /// <typeparam name="TArgument">The type of the recorded arguments.</typeparam>
    /// <param name="patternDelegate">Creates the pattern used to filter arguments before invoking recorders.</param>
    /// <param name="semanticRecorder">The recorder responsible for recording arguments of the constructor parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// </list></param>
    /// <param name="syntacticRecorder">The recorder responsible for recording syntactic information about arguments of the constructor parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The syntactic description of the argument.</item>
    /// </list></param>
    /// <returns>The created provider.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract IDetachedMappedSplitConstructorArgumentRecorderProvider<TSemanticRecord, TSyntacticRecord> Create<TArgument>(Func<IArgumentPatternFactory, IArgumentPattern<TArgument>> patternDelegate, Action<TSemanticRecord, TArgument> semanticRecorder, Action<TSyntacticRecord, ExpressionSyntax> syntacticRecorder);
}
