namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System;

/// <summary>Responsible for mapping attribute parameters to <see cref="DSemanticAttributeArgumentRecorder"/>, responsible for recording the argument of the parameter.</summary>
/// <typeparam name="TData">The type to which the produced <see cref="DSemanticAttributeArgumentRecorder"/> records the arguments of attribute parameters.</typeparam>
public interface ISemanticAttributeMapper<in TData>
{
    /// <summary>Attempts to map the provided <see cref="ITypeParameterSymbol"/> to a <see cref="DSemanticAttributeArgumentRecorder"/>, responsible for recording the argument of the attribute type-parameter.</summary>
    /// <param name="dataRecord">The <typeparamref name="TData"/> to which the produced <see cref="DSemanticAttributeArgumentRecorder"/> records the argument of the attribute parameter.</param>
    /// <param name="parameter">The <see cref="ITypeParameterSymbol"/> representing the type-parameter, the arguments of which is being recorded by the produced <see cref="DSemanticAttributeArgumentRecorder"/>.</param>
    /// <returns>The <see cref="DSemanticAttributeArgumentRecorder"/>, responsible for recording arguments of the specified type-parameter to the provided <typeparamref name="TData"/>.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract DSemanticAttributeArgumentRecorder? TryMapTypeParameter(TData dataRecord, ITypeParameterSymbol parameter);

    /// <summary>Attempts to map the provided <see cref="IParameterSymbol"/> to a <see cref="DSemanticAttributeArgumentRecorder"/>, responsible for recording the argument of the attribute parameter.</summary>
    /// <param name="dataRecord">The <typeparamref name="TData"/> to which the produced <see cref="DSemanticAttributeArgumentRecorder"/> records the argument of the attribute parameter.</param>
    /// <param name="parameter">The <see cref="IParameterSymbol"/> representing the parameter, the arguments of which is being recorded by the produced <see cref="DSemanticAttributeArgumentRecorder"/>.</param>
    /// <returns>The <see cref="DSemanticAttributeArgumentRecorder"/>, responsible for recording arguments of the specified parameter to the provided <typeparamref name="TData"/>.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract DSemanticAttributeArgumentRecorder? TryMapConstructorParameter(TData dataRecord, IParameterSymbol parameter);

    /// <summary>Attempts to map the provided parameter name to a <see cref="DSemanticAttributeArgumentRecorder"/>, responsible for recording the argument of the attribute parameter.</summary>
    /// <param name="dataRecord">The <typeparamref name="TData"/> to which the produced <see cref="DSemanticAttributeArgumentRecorder"/> records the argument of the attribute parameter.</param>
    /// <param name="parameterName">The name of the parameter, the arguments of which is being recorded by the produced <see cref="DSemanticAttributeArgumentRecorder"/>.</param>
    /// <returns>The <see cref="DSemanticAttributeArgumentRecorder"/>, responsible for recording arguments of the specified parameter to the provided <typeparamref name="TData"/>.</returns>
    /// <exception cref="ArgumentNullException"/>
    public abstract DSemanticAttributeArgumentRecorder? TryMapNamedParameter(TData dataRecord, string parameterName);
}
