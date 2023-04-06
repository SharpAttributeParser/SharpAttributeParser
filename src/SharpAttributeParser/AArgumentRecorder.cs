namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;

/// <summary>An abstract <see cref="ISyntacticArgumentRecorder"/> and <see cref="ISemanticArgumentRecorder"/>, recording parsed attribute arguments using delegates accessed through <see cref="string"/>-dictionaries.</summary>
/// <remarks>Mappings from parameter names to delegates are added by overriding the following methods:
/// <list type="bullet">
/// <item><see cref="ASyntacticArgumentRecorder.AddGenericRecorders"/></item>
/// <item><see cref="ASyntacticArgumentRecorder.AddSingleRecorders"/></item>
/// <item><see cref="ASyntacticArgumentRecorder.AddArrayRecorders"/></item>
/// </list></remarks>
public abstract class AArgumentRecorder : ASyntacticArgumentRecorder, ISemanticArgumentRecorder
{
    /// <inheritdoc/>
    public bool TryRecordGenericArgument(ITypeParameterSymbol parameter, ITypeSymbol value) => TryRecordGenericArgument(parameter, value, Location.None);

    /// <inheritdoc/>
    public bool TryRecordConstructorArgument(IParameterSymbol parameter, object? value) => TryRecordConstructorArgument(parameter, value, Location.None);

    /// <inheritdoc/>
    public bool TryRecordConstructorArgument(IParameterSymbol parameter, IReadOnlyList<object?>? value) => TryRecordConstructorArgument(parameter, value, Location.None, Array.Empty<Location>());

    /// <inheritdoc/>
    public bool TryRecordNamedArgument(string parameterName, object? value) => TryRecordNamedArgument(parameterName, value, Location.None);

    /// <inheritdoc/>
    public bool TryRecordNamedArgument(string parameterName, IReadOnlyList<object?>? value) => TryRecordNamedArgument(parameterName, value, Location.None, Array.Empty<Location>());
}
