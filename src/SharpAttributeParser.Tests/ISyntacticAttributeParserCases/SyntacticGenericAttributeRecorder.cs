namespace SharpAttributeParser.Tests.ISyntacticAttributeParserCases;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;

public sealed class SyntacticGenericAttributeRecorder : ASyntacticArgumentRecorder
{
    public ITypeSymbol? T { get; private set; }
    public Location? TLocation { get; private set; }

    protected override IEnumerable<(string, DSyntacticGenericRecorder)> AddGenericRecorders()
    {
        yield return ("T", RecordT);
    }

    private bool RecordT(ITypeSymbol value, Location location)
    {
        T = value;
        TLocation = location;

        return true;
    }
}
