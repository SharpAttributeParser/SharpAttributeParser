namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System;

/// <summary>Maps attribute parameters to <see cref="ISyntacticAttributeArgumentRecorder"/>, responsible for recording syntactical information about arguments of that parameter.</summary>
/// <typeparam name="TRecord">The type to which the mapped <see cref="ISyntacticAttributeArgumentRecorder"/> records syntactical information.</typeparam>
public interface ISyntacticAttributeMapper<in TRecord>
{
    /// <summary>Attempts to map the provided <see cref="ITypeParameterSymbol"/> to a <see cref="ISyntacticAttributeArgumentRecorder"/>.</summary>
    /// <param name="parameter">The <see cref="ITypeParameterSymbol"/> representing the type-parameter.</param>
    /// <param name="dataRecord">The <typeparamref name="TRecord"/> to which the mapped <see cref="ISyntacticAttributeArgumentRecorder"/> records syntactical information.</param>
    /// <returns>The mapped <see cref="ISyntacticAttributeArgumentRecorder"/>, or <see langword="null"/> if the attempt was unsuccessful.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract ISyntacticAttributeArgumentRecorder? TryMapTypeParameter(ITypeParameterSymbol parameter, TRecord dataRecord);

    /// <summary>Attempts to map the provided <see cref="IParameterSymbol"/> to a <see cref="ISyntacticAttributeConstructorArgumentRecorder"/>.</summary>
    /// <param name="parameter">The <see cref="IParameterSymbol"/> representing the parameter.</param>
    /// <param name="dataRecord">The <typeparamref name="TRecord"/> to which the mapped <see cref="ISyntacticAttributeConstructorArgumentRecorder"/> records syntactical information.</param>
    /// <returns>The mapped <see cref="ISyntacticAttributeConstructorArgumentRecorder"/>, or <see langword="null"/> if the attempt was unsuccessful.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract ISyntacticAttributeConstructorArgumentRecorder? TryMapConstructorParameter(IParameterSymbol parameter, TRecord dataRecord);

    /// <summary>Attempts to map the provided parameter name to a <see cref="ISyntacticAttributeArgumentRecorder"/>.</summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="dataRecord">The <typeparamref name="TRecord"/> to which the mapped <see cref="ISyntacticAttributeArgumentRecorder"/> records syntactical information.</param>
    /// <returns>The mapped <see cref="ISyntacticAttributeArgumentRecorder"/>, or <see langword="null"/> if the attempt was unsuccessful.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract ISyntacticAttributeArgumentRecorder? TryMapNamedParameter(string parameterName, TRecord dataRecord);
}
