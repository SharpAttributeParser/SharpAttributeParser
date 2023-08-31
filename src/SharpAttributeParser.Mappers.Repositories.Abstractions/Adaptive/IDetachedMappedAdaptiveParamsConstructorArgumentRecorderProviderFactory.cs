namespace SharpAttributeParser.Mappers.Repositories.Adaptive;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using SharpAttributeParser.Patterns;

using System;
using System.Collections.Generic;

/// <summary>Handles creation of <see cref="IDetachedMappedAdaptiveConstructorArgumentRecorderProvider{TCombinedRecord, TSemanticRecord}"/> related to <see langword="params"/> constructor parameters.</summary>
/// <typeparam name="TCombinedRecord">The type to which arguments are recorded when parsed with syntactic context.</typeparam>
/// <typeparam name="TSemanticRecord">The type to which arguments are recorded when parsed without syntactic context.</typeparam>
public interface IDetachedMappedAdaptiveParamsConstructorArgumentRecorderProviderFactory<TCombinedRecord, TSemanticRecord>
{
    /// <summary>Creates a provider of recorders which invoke the provided recorders.</summary>
    /// <param name="combinedRecorder">The recorder responsible for recording arguments of the constructor parameter when parsed with syntactic context. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// <item>The syntactic description of the argument, or of each element if the argument was specified as a <see langword="params"/>-array.</item>
    /// </list>
    /// The <see cref="bool"/> returned by the recorder should indicate whether the argument was successfully recorded.
    /// </param>
    /// <param name="semanticRecorder">The recorder responsible for recording arguments of the constructor parameter when parsed without syntactic context. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// </list>
    /// The <see cref="bool"/> returned by the recorder should indicate whether the argument was successfully recorded.
    /// </param>
    /// <returns>The created provider.</returns>
    public abstract IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord> Create(Func<TCombinedRecord, object?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> combinedRecorder, Func<TSemanticRecord, object?, bool> semanticRecorder);

    /// <summary>Creates a provider of recorders which invoke the provided recorders and return <see langword="true"/>.</summary>
    /// <param name="combinedRecorder">The recorder responsible for recording arguments of the constructor parameter when parsed with syntactic context. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// <item>The syntactic description of the argument, or of each element if the argument was specified as a <see langword="params"/>-array.</item>
    /// </list></param>
    /// <param name="semanticRecorder">The recorder responsible for recording arguments of the constructor parameter when parsed without syntactic context. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// </list></param>
    /// <returns>The created provider.</returns>
    public abstract IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord> Create(Action<TCombinedRecord, object?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> combinedRecorder, Action<TSemanticRecord, object?> semanticRecorder);

    /// <summary>Creates a provider of recorders which filter arguments using the provided pattern before invoking the provided recorders, and which return <see langword="false"/> for discarded arguments.</summary>
    /// <typeparam name="TArgument">The type of the recorded arguments.</typeparam>
    /// <param name="pattern">The pattern used to filter arguments before invoking recorders.</param>
    /// <param name="combinedRecorder">The recorder responsible for recording arguments of the constructor parameter when parsed with syntactic context. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// <item>The syntactic description of the argument, or of each element if the argument was specified as a <see langword="params"/>-array.</item>
    /// </list>
    /// The <see cref="bool"/> returned by the recorder should indicate whether the argument was successfully recorded.
    /// </param>
    /// <param name="semanticRecorder">The recorder responsible for recording arguments of the constructor parameter when parsed without syntactic context. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// </list>
    /// The <see cref="bool"/> returned by the recorder should indicate whether the argument was successfully recorded.
    /// </param>
    /// <returns>The created provider.</returns>
    public abstract IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord> Create<TArgument>(IArgumentPattern<TArgument> pattern, Func<TCombinedRecord, TArgument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> combinedRecorder, Func<TSemanticRecord, TArgument, bool> semanticRecorder);

    /// <summary>Creates a provider of recorders which filter arguments using the provided pattern before invoking the provided recorders and returning <see langword="true"/>, and which return <see langword="false"/> for discarded arguments.</summary>
    /// <typeparam name="TArgument">The type of the recorded arguments.</typeparam>
    /// <param name="pattern">The pattern used to filter arguments before invoking recorders.</param>
    /// <param name="combinedRecorder">The recorder responsible for recording arguments of the constructor parameter when parsed with syntactic context. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// <item>The syntactic description of the argument, or of each element if the argument was specified as a <see langword="params"/>-array.</item>
    /// </list></param>
    /// <param name="semanticRecorder">The recorder responsible for recording arguments of the constructor parameter when parsed without syntactic context. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// </list></param>
    /// <returns>The created provider.</returns>
    public abstract IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord> Create<TArgument>(IArgumentPattern<TArgument> pattern, Action<TCombinedRecord, TArgument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> combinedRecorder, Action<TSemanticRecord, TArgument> semanticRecorder);

    /// <summary>Creates a provider of recorders which filter arguments using the provided pattern before invoking the provided recorders, and which return <see langword="false"/> for discarded arguments.</summary>
    /// <typeparam name="TArgument">The type of the recorded arguments.</typeparam>
    /// <param name="patternDelegate">Creates the pattern used to filter arguments before invoking recorders.</param>
    /// <param name="combinedRecorder">The recorder responsible for recording arguments of the constructor parameter when parsed with syntactic context. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// <item>The syntactic description of the argument, or of each element if the argument was specified as a <see langword="params"/>-array.</item>
    /// </list>
    /// The <see cref="bool"/> returned by the recorder should indicate whether the argument was successfully recorded.
    /// </param>
    /// <param name="semanticRecorder">The recorder responsible for recording arguments of the constructor parameter when parsed without syntactic context. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// </list>
    /// The <see cref="bool"/> returned by the recorder should indicate whether the argument was successfully recorded.
    /// </param>
    /// <returns>The created provider.</returns>
    public abstract IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord> Create<TArgument>(Func<IArgumentPatternFactory, IArgumentPattern<TArgument>> patternDelegate, Func<TCombinedRecord, TArgument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> combinedRecorder, Func<TSemanticRecord, TArgument, bool> semanticRecorder);

    /// <summary>Creates a provider of recorders which filter arguments using the provided pattern before invoking the provided recorders and returning <see langword="true"/>, and which return <see langword="false"/> for discarded arguments.</summary>
    /// <typeparam name="TArgument">The type of the recorded arguments.</typeparam>
    /// <param name="patternDelegate">Creates the pattern used to filter arguments before invoking recorders.</param>
    /// <param name="combinedRecorder">The recorder responsible for recording arguments of the constructor parameter when parsed with syntactic context. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// <item>The syntactic description of the argument, or of each element if the argument was specified as a <see langword="params"/>-array.</item>
    /// </list></param>
    /// <param name="semanticRecorder">The recorder responsible for recording arguments of the constructor parameter when parsed without syntactic context. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// </list></param>
    /// <returns>The created provider.</returns>
    public abstract IDetachedMappedAdaptiveConstructorArgumentRecorderProvider<TCombinedRecord, TSemanticRecord> Create<TArgument>(Func<IArgumentPatternFactory, IArgumentPattern<TArgument>> patternDelegate, Action<TCombinedRecord, TArgument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> combinedRecorder, Action<TSemanticRecord, TArgument> semanticRecorder);
}
