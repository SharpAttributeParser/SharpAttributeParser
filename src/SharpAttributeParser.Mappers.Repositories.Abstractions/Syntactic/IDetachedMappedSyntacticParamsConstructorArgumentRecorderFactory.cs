namespace SharpAttributeParser.Mappers.Repositories.Syntactic;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using System;
using System.Collections.Generic;

/// <summary>Handles creation of <see cref="IDetachedMappedSyntacticConstructorArgumentRecorder{TRecord}"/> related to <see langword="params"/> constructor parameters.</summary>
/// <typeparam name="TRecord">The type to which syntactic information about arguments is recorded.</typeparam>
public interface IDetachedMappedSyntacticParamsConstructorArgumentRecorderFactory<TRecord>
{
    /// <summary>Creates a recorder which invokes the provided recorder.</summary>
    /// <param name="recorder">The recorder responsible for recording syntactic information about the arguments of the constructor parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the syntactic information is recorded.</item>
    /// <item>The syntactic description of the argument, or of each element if the argument was specified as a <see langword="params"/>-array.</item>
    /// </list>
    /// The <see cref="bool"/> returned by the recorder should indicate whether syntactic information was successfully recorded.
    /// </param>
    /// <returns>The created recorder.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord> Create(Func<TRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> recorder);

    /// <summary>Creates a recorder which invokes the provided recorder and returns <see langword="true"/>.</summary>
    /// <param name="recorder">The recorder responsible for recording syntactic information about the arguments of the constructor parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the syntactic information is recorded.</item>
    /// <item>The syntactic description of the argument, or of each element if the argument was specified as a <see langword="params"/>-array.</item>
    /// </list></param>
    /// <returns>The created recorder.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord> Create(Action<TRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>> recorder);
}
