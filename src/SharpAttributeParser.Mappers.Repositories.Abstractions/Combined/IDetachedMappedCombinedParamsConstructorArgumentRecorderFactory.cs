﻿namespace SharpAttributeParser.Mappers.Repositories.Combined;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using SharpAttributeParser.Patterns;

using System;
using System.Collections.Generic;

/// <summary>Handles creation of <see cref="IDetachedMappedCombinedConstructorArgumentRecorder{TRecord}"/> related to <see langword="params"/> constructor parameters.</summary>
/// <typeparam name="TRecord">The type to which arguments are recorded.</typeparam>
public interface IDetachedMappedCombinedParamsConstructorArgumentRecorderFactory<TRecord>
{
    /// <summary>Creates a recorder which invokes the provided recorder.</summary>
    /// <param name="recorder">The recorder responsible for recording arguments of the constructor parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// <item>The syntactic description of the argument, or of each element if the argument was specified as a <see langword="params"/>-array.</item>
    /// </list>
    /// The <see cref="bool"/> returned by the recorder should indicate whether the argument was successfully recorded.
    /// </param>
    /// <returns>The created recorder.</returns>
    public abstract IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> Create(Func<TRecord, object?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> recorder);

    /// <summary>Creates a recorder which invokes the provided recorder and returns <see langword="true"/>.</summary>
    /// <param name="recorder">The recorder responsible for recording arguments of the constructor parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// <item>The syntactic description of the argument, or of each element if the argument was specified as a <see langword="params"/>-array.</item>
    /// </list></param>
    /// <returns>The created recorder.</returns>
    public abstract IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> Create(Action<TRecord, object?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> recorder);

    /// <summary>Creates a recorder which filters arguments using the provided pattern before invoking the provided recorder, and which returns <see langword="false"/> for discarded arguments.</summary>
    /// <typeparam name="TArgument">The type of the recorded arguments.</typeparam>
    /// <param name="pattern">The pattern used to filter arguments before invoking recorders.</param>
    /// <param name="recorder">The recorder responsible for recording arguments of the constructor parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// <item>The syntactic description of the argument, or of each element if the argument was specified as a <see langword="params"/>-array.</item>
    /// </list>
    /// The <see cref="bool"/> returned by the recorder should indicate whether the argument was successfully recorded.
    /// </param>
    /// <returns>The created recorder.</returns>
    public abstract IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> Create<TArgument>(IArgumentPattern<TArgument> pattern, Func<TRecord, TArgument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> recorder);

    /// <summary>Creates a recorder which filters arguments using the provided pattern before invoking the provided recorder and returning <see langword="true"/>, and which returns <see langword="false"/> for discarded arguments.</summary>
    /// <typeparam name="TArgument">The type of the recorded arguments.</typeparam>
    /// <param name="pattern">The pattern used to filter arguments before invoking recorders.</param>
    /// <param name="recorder">The recorder responsible for recording arguments of the constructor parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// <item>The syntactic description of the argument, or of each element if the argument was specified as a <see langword="params"/>-array.</item>
    /// </list></param>
    /// <returns>The created recorder.</returns>
    public abstract IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> Create<TArgument>(IArgumentPattern<TArgument> pattern, Action<TRecord, TArgument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> recorder);

    /// <summary>Creates a recorder which filters arguments using the provided pattern before invoking the provided recorder, and which returns <see langword="false"/> for discarded arguments.</summary>
    /// <typeparam name="TArgument">The type of the recorded arguments.</typeparam>
    /// <param name="patternDelegate">Creates the pattern used to filter arguments before invoking recorders.</param>
    /// <param name="recorder">The recorder responsible for recording arguments of the constructor parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// <item>The syntactic description of the argument, or of each element if the argument was specified as a <see langword="params"/>-array.</item>
    /// </list>
    /// The <see cref="bool"/> returned by the recorder should indicate whether the argument was successfully recorded.
    /// </param>
    /// <returns>The created recorder.</returns>
    public abstract IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> Create<TArgument>(Func<IArgumentPatternFactory, IArgumentPattern<TArgument>> patternDelegate, Func<TRecord, TArgument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> recorder);

    /// <summary>Creates a recorder which filters arguments using the provided pattern before invoking the provided recorder and returning <see langword="true"/>, and which returns <see langword="false"/> for discarded arguments.</summary>
    /// <typeparam name="TArgument">The type of the recorded arguments.</typeparam>
    /// <param name="patternDelegate">Creates the pattern used to filter arguments before invoking recorders.</param>
    /// <param name="recorder">The recorder responsible for recording arguments of the constructor parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the argument is recorded.</item>
    /// <item>The argument that is recorded.</item>
    /// <item>The syntactic description of the argument, or of each element if the argument was specified as a <see langword="params"/>-array.</item>
    /// </list></param>
    /// <returns>The created recorder.</returns>
    public abstract IDetachedMappedCombinedConstructorArgumentRecorder<TRecord> Create<TArgument>(Func<IArgumentPatternFactory, IArgumentPattern<TArgument>> patternDelegate, Action<TRecord, TArgument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> recorder);
}
