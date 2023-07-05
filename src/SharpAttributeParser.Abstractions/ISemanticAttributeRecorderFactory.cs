namespace SharpAttributeParser;

using System;

/// <summary>Handles production of <see cref="ISemanticAttributeRecorder"/> using <see cref="ISemanticAttributeMapper{TData}"/>.</summary>
public interface ISemanticAttributeRecorderFactory
{
    /// <summary>Produces a <see cref="ISemanticAttributeRecorder"/>, recording attribute arguments to the provided <typeparamref name="TData"/>.</summary>
    /// <typeparam name="TData">The type to which the produced <see cref="ISemanticAttributeRecorder"/> records attribute arguments.</typeparam>
    /// <param name="argumentMapper"><inheritdoc cref="ISemanticAttributeMapper{TData}" path="/summary"/></param>
    /// <param name="dataRecord">The <typeparamref name="TData"/> to which the produced <see cref="ISemanticAttributeRecorder"/> records attribute arguments.</param>
    /// <returns>The produced <see cref="ISemanticAttributeRecorder"/>.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract ISemanticAttributeRecorder<TData> Create<TData>(ISemanticAttributeMapper<TData> argumentMapper, TData dataRecord);

    /// <summary>Produces a <see cref="ISemanticAttributeRecorder"/>, recording attribute arguments using the provided <typeparamref name="TDataBuilder"/>.</summary>
    /// <typeparam name="TData">The type representing the recorded attribute arguments, when built by the provided <typeparamref name="TDataBuilder"/>.</typeparam>
    /// <typeparam name="TDataBuilder">The type to which the produced <see cref="ISemanticAttributeRecorder"/> records attribute arguments, and which can build a <typeparamref name="TData"/>.</typeparam>
    /// <param name="argumentMapper"><inheritdoc cref="ISemanticAttributeMapper{TData}" path="/summary"/></param>
    /// <param name="dataBuilder">The <typeparamref name="TDataBuilder"/> to which the produced <see cref="ISemanticAttributeRecorder"/> records attribute arguments, and which can build a <typeparamref name="TData"/>.</param>
    /// <returns>The produced <see cref="ISemanticAttributeRecorder"/>.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract ISemanticAttributeRecorder<TData> Create<TData, TDataBuilder>(ISemanticAttributeMapper<TDataBuilder> argumentMapper, TDataBuilder dataBuilder) where TDataBuilder : IAttributeDataBuilder<TData>;
}
