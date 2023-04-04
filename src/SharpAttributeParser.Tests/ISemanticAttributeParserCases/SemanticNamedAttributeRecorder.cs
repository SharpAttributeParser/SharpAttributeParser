namespace SharpAttributeParser.Tests.ISemanticAttributeParserCases;

using System.Collections.Generic;

public sealed class SemanticNamedAttributeRecorder : ASemanticArgumentRecorder
{
    public object? Value { get; private set; }
    public bool ValueRecorded { get; private set; }

    public IReadOnlyList<object?>? Values { get; private set; }
    public bool ValuesRecorded { get; private set; }

    protected override IEnumerable<(string, DSingleRecorder)> AddSingleRecorders()
    {
        yield return ("Value", RecordValue);
    }

    protected override IEnumerable<(string, DArrayRecorder)> AddArrayRecorders()
    {
        yield return ("Values", RecordValues);
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
