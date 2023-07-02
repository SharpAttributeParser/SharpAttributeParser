namespace SharpAttributeParser.ASemanticArgumentRecorderCases;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;

internal sealed class SemanticArgumentRecorder : ASemanticArgumentRecorder
{
    public ITypeSymbol? T { get; private set; }
    public bool TRecorded { get; private set; }

    public object? SingleValue { get; private set; }
    public bool SingleValueRecorded { get; private set; }

    public IReadOnlyList<object?>? ArrayValue { get; private set; }
    public bool ArrayValueRecorded { get; private set; }

    protected override IEqualityComparer<string> Comparer { get; }

    public SemanticArgumentRecorder(IEqualityComparer<string> comparer)
    {
        Comparer = comparer;
    }

    protected override IEnumerable<(int, DSemanticGenericRecorder)> AddIndexedGenericRecorders()
    {
        yield return (0, RecordT);
    }

    protected override IEnumerable<(string, DSemanticGenericRecorder)> AddNamedGenericRecorders()
    {
        yield return (string.Empty, RecordT);
    }

    protected override IEnumerable<(string, DSemanticSingleRecorder)> AddSingleRecorders()
    {
        yield return (string.Empty, RecordSingleValue);
    }

    protected override IEnumerable<(string, DSemanticArrayRecorder)> AddArrayRecorders()
    {
        yield return (string.Empty, RecordArrayValue);
    }

    private bool RecordT(ITypeSymbol value)
    {
        T = value;
        TRecorded = true;

        return true;
    }

    private bool RecordSingleValue(object? value)
    {
        SingleValue = value;
        SingleValueRecorded = true;

        return true;
    }

    private bool RecordArrayValue(IReadOnlyList<object?>? value)
    {
        ArrayValue = value;
        ArrayValueRecorded = true;

        return true;
    }
}
