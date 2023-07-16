namespace SharpAttributeParser.ExampleCases;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;

internal interface ISemanticExampleAttributeRecordBuilder : IRecordBuilder<ISemanticExampleAttributeRecord>
{
    public abstract void WithT(ITypeSymbol t);
    public abstract void WithSequence(IReadOnlyList<int> sequence);
    public abstract void WithName(string name);
    public abstract void WithAnswer(int answer);
}
