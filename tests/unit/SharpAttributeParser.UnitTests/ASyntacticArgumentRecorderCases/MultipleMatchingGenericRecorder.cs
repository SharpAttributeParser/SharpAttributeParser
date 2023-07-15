namespace SharpAttributeParser.ASyntacticArgumentRecorderCases;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;

internal sealed class MultipleMatchingGenericRecorder : ASyntacticArgumentRecorder
{
    public ITypeSymbol? T { get; private set; }
    public bool TRecorded { get; private set; }
    public Location? TLocation { get; private set; }

    protected override IEqualityComparer<string> Comparer { get; } = StringComparerMock.CreateComparer(true);

    protected override IEnumerable<(int, DSyntacticGenericRecorder)> AddIndexedGenericRecorders()
    {
        yield return (0, RecordTByIndex);
    }

    protected override IEnumerable<(string, DSyntacticGenericRecorder)> AddNamedGenericRecorders()
    {
        yield return (string.Empty, RecordTByName);
    }

    private bool RecordT(ITypeSymbol value, Location location)
    {
        T = value;
        TRecorded = true;
        TLocation = location;

        return true;
    }

    private bool RecordTByIndex(ITypeSymbol value, Location location) => RecordT(value, location);
    private bool RecordTByName(ITypeSymbol value, Location location) => RecordT(value, location);
}
