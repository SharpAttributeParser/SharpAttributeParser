namespace SharpAttributeParser.Mappers.Repositories.Semantic;

using SharpAttributeParser.Patterns;

using System;

/// <summary>Handles creation of <see cref="IDetachedMappedSemanticNamedArgumentRecorder{TRecord}"/>.</summary>
/// <typeparam name="TRecord">The type to which arguments are recorded.</typeparam>
public interface IDetachedMappedSemanticNamedArgumentRecorderFactory<TRecord>
{
    /// <summary>Creates a recorder which invokes the provided recorder.</summary>
    /// <param name="recorder">The recorder responsible for recording arguments of the named parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// </list>
    /// The <see cref="bool"/> returned by the recorder should indicate whether the argument was successfully recorded.
    /// </param>
    /// <returns>The created recorder.</returns>
    public abstract IDetachedMappedSemanticNamedArgumentRecorder<TRecord> Create(Func<TRecord, object?, bool> recorder);

    /// <summary>Creates a recorder which invokes the provided recorder and returns <see langword="true"/>.</summary>
    /// <param name="recorder">The recorder responsible for recording arguments of the named parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// </list></param>
    /// <returns>The created recorder.</returns>
    public abstract IDetachedMappedSemanticNamedArgumentRecorder<TRecord> Create(Action<TRecord, object?> recorder);

    /// <summary>Creates a recorder which filters arguments using the provided pattern before invoking the provided recorder, and which returns <see langword="false"/> for discarded arguments.</summary>
    /// <typeparam name="TArgument">The type of the recorded arguments.</typeparam>
    /// <param name="pattern">The pattern used to filter arguments before invoking recorders.</param>
    /// <param name="recorder">The recorder responsible for recording arguments of the named parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// </list>
    /// The <see cref="bool"/> returned by the recorder should indicate whether the argument was successfully recorded.
    /// </param>
    /// <returns>The created recorder.</returns>
    public abstract IDetachedMappedSemanticNamedArgumentRecorder<TRecord> Create<TArgument>(IArgumentPattern<TArgument> pattern, Func<TRecord, TArgument, bool> recorder);

    /// <summary>Creates a recorder which filters arguments using the provided pattern before invoking the provided recorder and returning <see langword="true"/>, and which returns <see langword="false"/> for discarded arguments.</summary>
    /// <typeparam name="TArgument">The type of the recorded arguments.</typeparam>
    /// <param name="pattern">The pattern used to filter arguments before invoking recorders.</param>
    /// <param name="recorder">The recorder responsible for recording arguments of the named parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// </list></param>
    /// <returns>The created recorder.</returns>
    public abstract IDetachedMappedSemanticNamedArgumentRecorder<TRecord> Create<TArgument>(IArgumentPattern<TArgument> pattern, Action<TRecord, TArgument> recorder);

    /// <summary>Creates a recorder which filters arguments using the provided pattern before invoking the provided recorder, and which returns <see langword="false"/> for discarded arguments.</summary>
    /// <typeparam name="TArgument">The type of the recorded arguments.</typeparam>
    /// <param name="patternDelegate">Creates the pattern used to filter arguments before invoking recorders.</param>
    /// <param name="recorder">The recorder responsible for recording arguments of the named parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// </list>
    /// The <see cref="bool"/> returned by the recorder should indicate whether the argument was successfully recorded.
    /// </param>
    /// <returns>The created recorder.</returns>
    public abstract IDetachedMappedSemanticNamedArgumentRecorder<TRecord> Create<TArgument>(Func<IArgumentPatternFactory, IArgumentPattern<TArgument>> patternDelegate, Func<TRecord, TArgument, bool> recorder);

    /// <summary>Creates a recorder which filters arguments using the provided pattern before invoking the provided recorder and returning <see langword="true"/>, and which returns <see langword="false"/> for discarded arguments.</summary>
    /// <typeparam name="TArgument">The type of the recorded arguments.</typeparam>
    /// <param name="patternDelegate">Creates the pattern used to filter arguments before invoking recorders.</param>
    /// <param name="recorder">The recorder responsible for recording arguments of the named parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// </list></param>
    /// <returns>The created recorder.</returns>
    public abstract IDetachedMappedSemanticNamedArgumentRecorder<TRecord> Create<TArgument>(Func<IArgumentPatternFactory, IArgumentPattern<TArgument>> patternDelegate, Action<TRecord, TArgument> recorder);
}
