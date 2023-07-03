namespace SharpAttributeParser;

using System;

/// <summary>Responsible for producing <see cref="ISemanticAttributeRecorder"/>.</summary>
public interface ISemanticAttributeRecorderFactory
{
    /// <summary>Produces a <see cref="ISemanticAttributeRecorder"/>, recording attribute arguments to the provided <typeparamref name="TData"/>.</summary>
    /// <typeparam name="TData">The type to which the produced <see cref="ISemanticAttributeRecorder"/> records attribute arguments.</typeparam>
    /// <param name="argumentMapper"><inheritdoc cref="ISemanticAttributeMapper{TData}" path="/summary"/></param>
    /// <param name="data">The <typeparamref name="TData"/> to which the produced <see cref="ISemanticAttributeRecorder"/> records attribute arguments.</param>
    /// <returns>The produced <see cref="ISemanticAttributeRecorder"/>.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract ISemanticAttributeRecorder Create<TData>(ISemanticAttributeMapper<TData> argumentMapper, TData data);
}
