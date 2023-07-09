namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System;

/// <summary>Maps attribute parameters to <see cref="IAttributeArgumentRecorder"/>, responsible for recording arguments of that parameter.</summary>
/// <typeparam name="TRecord">The type to which the mapped <see cref="IAttributeArgumentRecorder"/> records arguments and syntactical information.</typeparam>
public interface IAttributeMapper<in TRecord>
{
    /// <summary>Attempts to map the provided <see cref="ITypeParameterSymbol"/> to an <see cref="IAttributeArgumentRecorder"/>.</summary>
    /// <param name="parameter">The <see cref="ITypeParameterSymbol"/> representing the type-parameter.</param>
    /// <param name="dataRecord">The <typeparamref name="TRecord"/> to which the mapped <see cref="IAttributeArgumentRecorder"/> records arguments.</param>
    /// <returns>The mapped <see cref="IAttributeArgumentRecorder"/>, or <see langword="null"/> if the attempt was unsuccessful.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract IAttributeArgumentRecorder? TryMapTypeParameter(ITypeParameterSymbol parameter, TRecord dataRecord);

    /// <summary>Attempts to map the provided <see cref="IParameterSymbol"/> to an <see cref="IAttributeArgumentRecorder"/>.</summary>
    /// <param name="parameter">The <see cref="IParameterSymbol"/> representing the parameter.</param>
    /// <param name="dataRecord">The <typeparamref name="TRecord"/> to which the mapped <see cref="IAttributeArgumentRecorder"/> records arguments.</param>
    /// <returns>The mapped <see cref="IAttributeArgumentRecorder"/>, or <see langword="null"/> if the attempt was unsuccessful.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract IAttributeArgumentRecorder? TryMapConstructorParameter(IParameterSymbol parameter, TRecord dataRecord);

    /// <summary>Attempts to map the provided parameter name to an <see cref="IAttributeArgumentRecorder"/>.</summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="dataRecord">The <typeparamref name="TRecord"/> to which the mapped <see cref="IAttributeArgumentRecorder"/> records arguments.</param>
    /// <returns>The mapped <see cref="IAttributeArgumentRecorder"/>, or <see langword="null"/> if the attempt was unsuccessful.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract IAttributeArgumentRecorder? TryMapNamedParameter(string parameterName, TRecord dataRecord);
}
