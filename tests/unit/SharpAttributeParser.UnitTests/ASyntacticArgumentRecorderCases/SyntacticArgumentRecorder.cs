namespace SharpAttributeParser.ASyntacticArgumentRecorderCases;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;

internal sealed class SyntacticArgumentRecorder : ASyntacticArgumentRecorder
{
    public ITypeSymbol? T { get; private set; }
    public bool TRecorded { get; private set; }
    public Location? TLocation { get; private set; }

    public object? SingleValue { get; private set; }
    public bool SingleValueRecorded { get; private set; }
    public Location? ValueLocation { get; private set; }

    public IReadOnlyList<object?>? ArrayValue { get; private set; }
    public bool ArrayValueRecorded { get; private set; }
    public CollectionLocation? ArrayValueLocation { get; private set; }

    protected override IEqualityComparer<string> Comparer { get; }

    public SyntacticArgumentRecorder(IEqualityComparer<string> comparer)
    {
        Comparer = comparer;
    }

    protected override IEnumerable<(int, DSyntacticGenericRecorder)> AddIndexedGenericRecorders()
    {
        yield return (0, RecordT);
    }

    protected override IEnumerable<(string, DSyntacticGenericRecorder)> AddNamedGenericRecorders()
    {
        yield return (string.Empty, RecordT);
    }

    protected override IEnumerable<(string, DSyntacticSingleRecorder)> AddSingleRecorders()
    {
        yield return (string.Empty, RecordSingleValue);
    }

    protected override IEnumerable<(string, DSyntacticArrayRecorder)> AddArrayRecorders()
    {
        yield return (string.Empty, RecordArrayValue);
    }

    private bool RecordT(ITypeSymbol value, Location location)
    {
        T = value;
        TRecorded = true;
        TLocation = location;

        return true;
    }

    private bool RecordSingleValue(object? value, Location location)
    {
        SingleValue = value;
        SingleValueRecorded = true;
        ValueLocation = location;

        return true;
    }

    private bool RecordArrayValue(IReadOnlyList<object?>? value, CollectionLocation location)
    {
        ArrayValue = value;
        ArrayValueRecorded = true;
        ArrayValueLocation = location;

        return true;
    }
}
