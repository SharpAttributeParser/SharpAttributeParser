namespace SharpAttributeParser.Mappers.Repositories.Syntactic;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;
using OneOf.Types;

using System;

/// <summary>Handles creation of <see cref="IDetachedMappedSyntacticConstructorArgumentRecorder{TRecord}"/> related to optional constructor parameters.</summary>
/// <typeparam name="TRecord">The type to which syntactic information about arguments is recorded.</typeparam>
public interface IDetachedMappedSyntacticOptionalConstructorArgumentRecorderFactory<TRecord>
{
    /// <summary>Creates a recorder which invokes the provided recorder.</summary>
    /// <param name="recorder">The recorder responsible for recording syntactic information about the arguments of the constructor parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the syntactic information is recorded.</item>
    /// <item>The syntactic description of the argument, or <see cref="None"/> if the default argument of the optional parameter was used.</item>
    /// </list>
    /// The <see cref="bool"/> returned by the recorder should indicate whether syntactic information was successfully recorded.
    /// </param>
    /// <returns>The created recorder.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord> Create(Func<TRecord, OneOf<None, ExpressionSyntax>, bool> recorder);

    /// <summary>Creates a recorder which invokes the provided recorder and returns <see langword="true"/>.</summary>
    /// <param name="recorder">The recorder responsible for recording syntactic information about the arguments of the constructor parameter. The arguments provided to the recorder are:
    /// <list type="number">
    /// <item>The record to which the syntactic information is recorded.</item>
    /// <item>The syntactic description of the argument, or <see cref="None"/> if the default argument of the optional parameter was used.</item>
    /// </list></param>
    /// <returns>The created recorder.</returns>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public abstract IDetachedMappedSyntacticConstructorArgumentRecorder<TRecord> Create(Action<TRecord, OneOf<None, ExpressionSyntax>> recorder);
}
