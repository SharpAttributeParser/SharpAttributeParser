namespace SharpAttributeParser.Tests.ISyntacticAttributeParserCases;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;

public sealed class SyntacticSingleConstructorAttributeRecorder : ASyntacticArgumentRecorder
{
    public object? Value { get; private set; }
    public Location? ValueLocation { get; private set; }
    public bool ValueRecorded { get; private set; }

    protected override IEnumerable<(string, DSingleRecorder)> AddSingleRecorders()
    {
        yield return ("Value", RecordValue);
    }

    private bool RecordValue(object? value, Location location)
    {
        Value = value;
        ValueLocation = location;
        ValueRecorded = true;

        return true;
    }
}
