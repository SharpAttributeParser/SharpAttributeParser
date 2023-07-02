namespace SharpAttributeParser.ASemanticArgumentRecorderCases;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;

internal sealed class MultipleMatchingGenericRecorder : ASemanticArgumentRecorder
{
    public ITypeSymbol? T { get; private set; }
    public bool TRecorded { get; private set; }

    protected override IEqualityComparer<string> Comparer { get; } = StringComparerMock.CreateComparer(true);

    protected override IEnumerable<(int, DSemanticGenericRecorder)> AddIndexedGenericRecorders()
    {
        yield return (0, RecordTByIndex);
    }

    protected override IEnumerable<(string, DSemanticGenericRecorder)> AddNamedGenericRecorders()
    {
        yield return (string.Empty, RecordTByName);
    }

    private bool RecordT(ITypeSymbol value)
    {
        T = value;
        TRecorded = true;

        return true;
    }

    private bool RecordTByIndex(ITypeSymbol value) => RecordT(value);
    private bool RecordTByName(ITypeSymbol value) => RecordT(value);
}
