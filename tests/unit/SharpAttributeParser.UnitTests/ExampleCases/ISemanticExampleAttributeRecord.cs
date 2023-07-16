namespace SharpAttributeParser.ExampleCases;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;

public interface ISemanticExampleAttributeRecord
{
    public abstract ITypeSymbol T { get; }
    public abstract IReadOnlyList<int> Sequence { get; }
    public abstract string Name { get; }
    public abstract int? Answer { get; }
}
