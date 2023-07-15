namespace SharpAttributeParser.SyntacticAttributeParserCases;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;

public sealed class SyntacticGenericAttributeRecorder : ASyntacticArgumentRecorder
{
    public ITypeSymbol? T1 { get; private set; }
    public bool T1Recorded { get; private set; }
    public Location? T1Location { get; private set; }

    public ITypeSymbol? T2 { get; private set; }
    public bool T2Recorded { get; private set; }
    public Location? T2Location { get; private set; }

    protected override IEnumerable<(int, DSyntacticGenericRecorder)> AddIndexedGenericRecorders()
    {
        yield return (0, RecordT1);
        yield return (1, RecordT2);
    }

    private bool RecordT1(ITypeSymbol value, Location location)
    {
        T1 = value;
        T1Recorded = true;
        T1Location = location;

        return true;
    }

    private bool RecordT2(ITypeSymbol value, Location location)
    {
        T2 = value;
        T2Recorded = true;
        T2Location = location;

        return true;
    }
}
