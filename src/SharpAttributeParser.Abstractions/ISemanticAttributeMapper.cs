namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System;

/// <summary>Maps attribute parameters to <see cref="ISemanticAttributeArgumentRecorder"/>, responsible for recording arguments of that parameter.</summary>
/// <typeparam name="TRecord">The type to which the mapped <see cref="ISemanticAttributeArgumentRecorder"/> records arguments.</typeparam>
public interface ISemanticAttributeMapper<in TRecord>
{
    /// <summary>Attempts to map the provided <see cref="ITypeParameterSymbol"/> to a <see cref="ISemanticAttributeArgumentRecorder"/>.</summary>
    /// <param name="parameter">The <see cref="ITypeParameterSymbol"/> representing the type-parameter.</param>
    /// <param name="dataRecord">The <typeparamref name="TRecord"/> to which the mapped <see cref="ISemanticAttributeArgumentRecorder"/> records arguments.</param>
    /// <returns>The mapped <see cref="ISemanticAttributeArgumentRecorder"/>, or <see langword="null"/> if the attempt was unsuccessful.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract ISemanticAttributeArgumentRecorder? TryMapTypeParameter(ITypeParameterSymbol parameter, TRecord dataRecord);

    /// <summary>Attempts to map the provided <see cref="IParameterSymbol"/> to a <see cref="ISemanticAttributeArgumentRecorder"/>.</summary>
    /// <param name="parameter">The <see cref="IParameterSymbol"/> representing the parameter.</param>
    /// <param name="dataRecord">The <typeparamref name="TRecord"/> to which the mapped <see cref="ISemanticAttributeArgumentRecorder"/> records arguments.</param>
    /// <returns>The mapped <see cref="ISemanticAttributeArgumentRecorder"/>, or <see langword="null"/> if the attempt was unsuccessful.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract ISemanticAttributeArgumentRecorder? TryMapConstructorParameter(IParameterSymbol parameter, TRecord dataRecord);

    /// <summary>Attempts to map the provided parameter name to a <see cref="ISemanticAttributeArgumentRecorder"/>.</summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="dataRecord">The <typeparamref name="TRecord"/> to which the mapped <see cref="ISemanticAttributeArgumentRecorder"/> records arguments.</param>
    /// <returns>The mapped <see cref="ISemanticAttributeArgumentRecorder"/>, or <see langword="null"/> if the attempt was unsuccessful.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract ISemanticAttributeArgumentRecorder? TryMapNamedParameter(string parameterName, TRecord dataRecord);
}
