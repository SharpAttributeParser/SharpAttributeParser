namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System;

/// <inheritdoc cref="ISemanticAttributeRecorder{TData}"/>
internal sealed class SemanticAttributeRecorder<TData> : ISemanticAttributeRecorder<TData>
{
    private ISemanticAttributeMapper<TData> ArgumentMapper { get; }
    private TData AttributeData { get; }

    /// <summary>Instantiates a <see cref="SemanticAttributeRecorder{TData}"/>, recording the parsed of an attribute.</summary>
    /// <param name="argumentMapper"><inheritdoc cref="ISemanticAttributeMapper{TData}" path="/summary"/></param>
    /// <param name="attributeData">The <typeparamref name="TData"/> representing the recorded arguments.</param>
    /// <exception cref="ArgumentNullException"/>
    public SemanticAttributeRecorder(ISemanticAttributeMapper<TData> argumentMapper, TData attributeData)
    {
        ArgumentMapper = argumentMapper ?? throw new ArgumentNullException(nameof(argumentMapper));
        AttributeData = attributeData ?? throw new ArgumentNullException(nameof(attributeData));
    }

    /// <inheritdoc/>
    public TData GetResult() => AttributeData;

    /// <inheritdoc/>
    public bool TryRecordTypeArgument(ITypeParameterSymbol parameter, ITypeSymbol argument)
    {
        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        if (argument is null)
        {
            throw new ArgumentNullException(nameof(argument));
        }

        if (ArgumentMapper.TryMapTypeParameter(AttributeData, parameter) is not DSemanticAttributeArgumentRecorder recordDelegate)
        {
            return false;
        }

        return recordDelegate(argument);
    }

    /// <inheritdoc/>
    public bool TryRecordConstructorArgument(IParameterSymbol parameter, object? argument)
    {
        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        if (ArgumentMapper.TryMapConstructorParameter(AttributeData, parameter) is not DSemanticAttributeArgumentRecorder recorderDelegate)
        {
            return false;
        }

        return recorderDelegate(argument);
    }

    /// <inheritdoc/>
    public bool TryRecordNamedArgument(string parameterName, object? argument)
    {
        if (parameterName is null)
        {
            throw new ArgumentNullException(nameof(parameterName));
        }

        if (ArgumentMapper.TryMapNamedParameter(AttributeData, parameterName) is not DSemanticAttributeArgumentRecorder recorderDelegate)
        {
            return false;
        }

        return recorderDelegate(argument);
    }
}

/// <summary><inheritdoc cref="ISemanticAttributeRecorder{TData}" path="/summary"/></summary>
/// <typeparam name="TData">The type representing the recorded attribute arguments, when built by a <typeparamref name="TDataBuilder"/>.</typeparam>
/// <typeparam name="TDataBuilder">The type to which the <see cref="ISemanticAttributeRecorder"/> records attribute arguments, and which can build a <typeparamref name="TData"/>.</typeparam>
internal sealed class SemanticAttributeRecorder<TData, TDataBuilder> : ISemanticAttributeRecorder<TData> where TDataBuilder : IAttributeDataBuilder<TData>
{
    private ISemanticAttributeMapper<TDataBuilder> ArgumentMapper { get; }
    private TDataBuilder AttributeDataBuilder { get; }

    /// <summary>Instantiates a <see cref="SemanticAttributeRecorder{TData, TDataBuilder}"/>, recording the parsed of an attribute.</summary>
    /// <param name="argumentMapper"><inheritdoc cref="ISemanticAttributeMapper{TData}" path="/summary"/></param>
    /// <param name="attributeDataBuilder">The <typeparamref name="TDataBuilder"/> to which the produced <see cref="ISemanticAttributeRecorder"/> records attribute arguments, and which can build a <typeparamref name="TData"/>.</param>
    /// <exception cref="ArgumentNullException"/>
    public SemanticAttributeRecorder(ISemanticAttributeMapper<TDataBuilder> argumentMapper, TDataBuilder attributeDataBuilder)
    {
        ArgumentMapper = argumentMapper ?? throw new ArgumentNullException(nameof(argumentMapper));
        AttributeDataBuilder = attributeDataBuilder ?? throw new ArgumentNullException(nameof(attributeDataBuilder));
    }

    /// <inheritdoc/>
    public TData GetResult() => AttributeDataBuilder.Build();

    /// <inheritdoc/>
    public bool TryRecordTypeArgument(ITypeParameterSymbol parameter, ITypeSymbol argument)
    {
        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        if (argument is null)
        {
            throw new ArgumentNullException(nameof(argument));
        }

        if (ArgumentMapper.TryMapTypeParameter(AttributeDataBuilder, parameter) is not DSemanticAttributeArgumentRecorder recordDelegate)
        {
            return false;
        }

        return recordDelegate(argument);
    }

    /// <inheritdoc/>
    public bool TryRecordConstructorArgument(IParameterSymbol parameter, object? argument)
    {
        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        if (ArgumentMapper.TryMapConstructorParameter(AttributeDataBuilder, parameter) is not DSemanticAttributeArgumentRecorder recorderDelegate)
        {
            return false;
        }

        return recorderDelegate(argument);
    }

    /// <inheritdoc/>
    public bool TryRecordNamedArgument(string parameterName, object? argument)
    {
        if (parameterName is null)
        {
            throw new ArgumentNullException(nameof(parameterName));
        }

        if (ArgumentMapper.TryMapNamedParameter(AttributeDataBuilder, parameterName) is not DSemanticAttributeArgumentRecorder recorderDelegate)
        {
            return false;
        }

        return recorderDelegate(argument);
    }
}
