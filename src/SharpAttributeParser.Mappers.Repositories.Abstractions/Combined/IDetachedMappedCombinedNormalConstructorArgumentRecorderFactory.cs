namespace SharpAttributeParser.Mappers.Repositories.Combined;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser.Patterns;

using System;

/// <summary>Handles creation of <see cref="IDetachedMappedCombinedConstructorArgumentRecorder{TRecord}"/> related to non-optional, non-<see langword="params"/> constructor parameters.</summary>
/// <typeparam name="TRecord">The type to which arguments are recorded.</typeparam>
public interface IDetachedMappedCombinedNormalConstructorArgumentRecorderFactory<TRecord>
{
    /// <summary>Creates a recorder which invokes the provided recorder.</summary>
    /// <param name="recorder">The recorder responsible for recording arguments of the constructor parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// <item>The syntactic description of the argument.</item>
    /// </list>
    /// The <see cref="bool"/> returned by the recorder should indicate whether the argument was successfully recorded.
    /// </param>
    /// <returns>The created recorder.</returns>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> Create(Func<TRecord, object?, ExpressionSyntax, bool> recorder);

    /// <summary>Creates a recorder which invokes the provided recorder and returns <see langword="true"/>.</summary>
    /// <param name="recorder">The recorder responsible for recording arguments of the constructor parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// <item>The syntactic description of the argument.</item>
    /// </list></param>
    /// <returns>The created recorder.</returns>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> Create(Action<TRecord, object?, ExpressionSyntax> recorder);

    /// <summary>Creates a recorder which filters arguments using the provided pattern before invoking the provided recorder, and which returns <see langword="false"/> for discarded arguments.</summary>
    /// <typeparam name="TArgument">The type of the recorded arguments.</typeparam>
    /// <param name="pattern">The pattern used to filter arguments before invoking recorders.</param>
    /// <param name="recorder">The recorder responsible for recording arguments of the constructor parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// <item>The syntactic description of the argument.</item>
    /// </list>
    /// The <see cref="bool"/> returned by the recorder should indicate whether the argument was successfully recorded.
    /// </param>
    /// <returns>The created recorder.</returns>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> Create<TArgument>(IArgumentPattern<TArgument> pattern, Func<TRecord, TArgument, ExpressionSyntax, bool> recorder);

    /// <summary>Creates a recorder which filters arguments using the provided pattern before invoking the provided recorder and returning <see langword="true"/>, and which returns <see langword="false"/> for discarded arguments.</summary>
    /// <typeparam name="TArgument">The type of the recorded arguments.</typeparam>
    /// <param name="pattern">The pattern used to filter arguments before invoking recorders.</param>
    /// <param name="recorder">The recorder responsible for recording arguments of the constructor parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// <item>The syntactic description of the argument.</item>
    /// </list></param>
    /// <returns>The created recorder.</returns>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> Create<TArgument>(IArgumentPattern<TArgument> pattern, Action<TRecord, TArgument, ExpressionSyntax> recorder);

    /// <summary>Creates a recorder which filters arguments using the provided pattern before invoking the provided recorder, and which returns <see langword="false"/> for discarded arguments.</summary>
    /// <typeparam name="TArgument">The type of the recorded arguments.</typeparam>
    /// <param name="patternDelegate">Creates the pattern used to filter arguments before invoking recorders.</param>
    /// <param name="recorder">The recorder responsible for recording arguments of the constructor parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// <item>The syntactic description of the argument.</item>
    /// </list>
    /// The <see cref="bool"/> returned by the recorder should indicate whether the argument was successfully recorded.
    /// </param>
    /// <returns>The created recorder.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> Create<TArgument>(Func<IArgumentPatternFactory, IArgumentPattern<TArgument>> patternDelegate, Func<TRecord, TArgument, ExpressionSyntax, bool> recorder);

    /// <summary>Creates a recorder which filters arguments using the provided pattern before invoking the provided recorder and returning <see langword="true"/>, and which returns <see langword="false"/> for discarded arguments.</summary>
    /// <typeparam name="TArgument">The type of the recorded arguments.</typeparam>
    /// <param name="patternDelegate">Creates the pattern used to filter arguments before invoking recorders.</param>
    /// <param name="recorder">The recorder responsible for recording arguments of the constructor parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// <item>The syntactic description of the argument.</item>
    /// </list></param>
    /// <returns>The created recorder.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> Create<TArgument>(Func<IArgumentPatternFactory, IArgumentPattern<TArgument>> patternDelegate, Action<TRecord, TArgument, ExpressionSyntax> recorder);
}
