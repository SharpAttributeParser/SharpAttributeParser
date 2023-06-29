namespace SharpAttributeParser.Tests.ASemanticArgumentRecorderCases;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;

internal sealed class SemanticArgumentRecorder : ASemanticArgumentRecorder
{
    public ITypeSymbol? TGeneric { get; private set; }
    public object? Value { get; private set; }
    public IReadOnlyList<object?>? Values { get; private set; }

    public bool ValueRecorded { get; private set; }
    public bool ValuesRecorded { get; private set; }

    protected override IEqualityComparer<string> Comparer { get; }

    public SemanticArgumentRecorder(IEqualityComparer<string> comparer)
    {
        Comparer = comparer;
    }

    protected override IEnumerable<(string, DSemanticGenericRecorder)> AddGenericRecorders()
    {
        yield return ("TGeneric", RecordTGeneric);
    }

    protected override IEnumerable<(string, DSemanticSingleRecorder)> AddSingleRecorders()
    {
        yield return ("Value", RecordValue);
    }

    protected override IEnumerable<(string, DSemanticArrayRecorder)> AddArrayRecorders()
    {
        yield return ("Values", RecordValues);
    }

    private bool RecordTGeneric(ITypeSymbol value)
    {
        TGeneric = value;

        return true;
    }

    private bool RecordValue(object? value)
    {
        Value = value;
        ValueRecorded = true;

        return true;
    }

    private bool RecordValues(IReadOnlyList<object?>? values)
    {
        Values = values;
        ValuesRecorded = true;

        return true;
    }
}
