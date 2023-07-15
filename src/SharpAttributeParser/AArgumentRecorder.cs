namespace SharpAttributeParser;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;

/// <summary>An abstract <see cref="ISyntacticArgumentRecorder"/> and <see cref="ISemanticArgumentRecorder"/>, recording parsed attribute arguments using recorders provided through the following methods:
/// <list type="bullet">
/// <item><see cref="ASyntacticArgumentRecorder.AddIndexedGenericRecorders"/></item>
/// <item><see cref="ASyntacticArgumentRecorder.AddNamedGenericRecorders"/></item>
/// <item><see cref="ASyntacticArgumentRecorder.AddSingleRecorders"/></item>
/// <item><see cref="ASyntacticArgumentRecorder.AddArrayRecorders"/></item>
/// </list></summary>
public abstract class AArgumentRecorder : ASyntacticArgumentRecorder, ISemanticArgumentRecorder
{
    /// <inheritdoc/>
    public bool TryRecordGenericArgument(ITypeParameterSymbol parameter, ITypeSymbol value) => TryRecordGenericArgument(parameter, value, Location.None);

    /// <inheritdoc/>
    public bool TryRecordConstructorArgument(IParameterSymbol parameter, object? value) => TryRecordConstructorArgument(parameter, value, Location.None);

    /// <inheritdoc/>
    public bool TryRecordConstructorArgument(IParameterSymbol parameter, IReadOnlyList<object?>? value) => TryRecordConstructorArgument(parameter, value, CollectionLocation.None);

    /// <inheritdoc/>
    public bool TryRecordNamedArgument(string parameterName, object? value) => TryRecordNamedArgument(parameterName, value, Location.None);

    /// <inheritdoc/>
    public bool TryRecordNamedArgument(string parameterName, IReadOnlyList<object?>? value) => TryRecordNamedArgument(parameterName, value, CollectionLocation.None);
}
