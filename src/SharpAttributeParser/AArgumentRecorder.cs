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
    public bool TryRecordGenericArgument(string parameterName, ITypeSymbol value) => TryRecordGenericArgument(parameterName, value, Location.None);

    /// <inheritdoc/>
    public bool TryRecordConstructorArgument(string parameterName, object? value) => TryRecordConstructorArgument(parameterName, value, Location.None);

    /// <inheritdoc/>
    public bool TryRecordConstructorArgument(string parameterName, IReadOnlyList<object?>? value) => TryRecordConstructorArgument(parameterName, value, Location.None, Array.Empty<Location>());

    /// <inheritdoc/>
    public bool TryRecordNamedArgument(string parameterName, object? value) => TryRecordNamedArgument(parameterName, value, Location.None);

    /// <inheritdoc/>
    public bool TryRecordNamedArgument(string parameterName, IReadOnlyList<object?>? value) => TryRecordNamedArgument(parameterName, value, Location.None, Array.Empty<Location>());
}
