namespace SharpAttributeParser.SyntacticAttributeParserCases;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;

public sealed class SyntacticConstructorSingleAttributeRecorder : ASyntacticArgumentRecorder
{
    public object? Value { get; private set; }
    public bool ValueRecorded { get; private set; }
    public Location? ValueLocation { get; private set; }

    protected override IEnumerable<(string, DSyntacticSingleRecorder)> AddSingleRecorders()
    {
        yield return (nameof(ConstructorSingleAttribute.Value), RecordValue);
    }

    private bool RecordValue(object? value, Location location)
    {
        Value = value;
        ValueRecorded = true;
        ValueLocation = location;

        return true;
    }
}
