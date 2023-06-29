namespace SharpAttributeParser.Tests.ISemanticAttributeParserCases;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;

public sealed class SemanticGenericAttributeRecorder : ASemanticArgumentRecorder
{
    public ITypeSymbol? T { get; private set; }

    protected override IEnumerable<(string, DSemanticGenericRecorder)> AddGenericRecorders()
    {
        yield return ("T", RecordT);
    }

    private bool RecordT(ITypeSymbol value)
    {
        T = value;

        return true;
    }
}
