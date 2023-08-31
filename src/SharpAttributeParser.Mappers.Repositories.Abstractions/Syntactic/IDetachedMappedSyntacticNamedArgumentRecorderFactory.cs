﻿namespace SharpAttributeParser.Mappers.Repositories.Syntactic;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;

/// <summary>Handles creation of <see cref="IDetachedMappedSyntacticNamedArgumentRecorder{TRecord}"/>.</summary>
/// <typeparam name="TRecord">The type to which syntactic information about arguments is recorded.</typeparam>
public interface IDetachedMappedSyntacticNamedArgumentRecorderFactory<TRecord>
{
    /// <summary>Creates a recorder which invokes the provided recorder.</summary>
    /// <param name="recorder">The recorder responsible for recording syntactic information about the arguments of the named parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the syntactic information is recorded.</item>
    /// <item>The syntactic description of the argument.</item>
    /// </list>
    /// The <see cref="bool"/> returned by the recorder should indicate whether syntactic information was successfully recorded.
    /// </param>
    /// <returns>The created recorder.</returns>
    public abstract IDetachedMappedSyntacticNamedArgumentRecorder<TRecord> Create(Func<TRecord, ExpressionSyntax, bool> recorder);

    /// <summary>Creates a recorder which invokes the provided recorder and returns <see langword="true"/>.</summary>
    /// <param name="recorder">The recorder responsible for recording syntactic information about the arguments of the named parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the syntactic information is recorded.</item>
    /// <item>The syntactic description of the argument.</item>
    /// </list></param>
    /// <returns>The created recorder.</returns>
    public abstract IDetachedMappedSyntacticNamedArgumentRecorder<TRecord> Create(Action<TRecord, ExpressionSyntax> recorder);
}
