namespace SharpAttributeParser.SemanticAttributeParserCases;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;

public sealed class SemanticGenericAttributeRecorder : ASemanticArgumentRecorder
{
    public ITypeSymbol? T1 { get; private set; }
    public ITypeSymbol? T2 { get; private set; }

    public bool T1Recorded { get; private set; }
    public bool T2Recorded { get; private set; }

    protected override IEnumerable<(int, DSemanticGenericRecorder)> AddIndexedGenericRecorders()
    {
        yield return (0, RecordT1);
        yield return (1, RecordT2);
    }

    private bool RecordT1(ITypeSymbol value)
    {
        T1 = value;
        T1Recorded = true;

        return true;
    }

    private bool RecordT2(ITypeSymbol value)
    {
        T2 = value;
        T2Recorded = true;

        return true;
    }
}
