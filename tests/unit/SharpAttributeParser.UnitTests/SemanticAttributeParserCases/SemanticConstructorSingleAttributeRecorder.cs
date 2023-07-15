namespace SharpAttributeParser.SemanticAttributeParserCases;

using SharpAttributeParser;

using System.Collections.Generic;

public sealed class SemanticConstructorSingleAttributeRecorder : ASemanticArgumentRecorder
{
    public object? Value { get; private set; }
    public bool ValueRecorded { get; private set; }

    protected override IEnumerable<(string, DSemanticSingleRecorder)> AddSingleRecorders()
    {
        yield return (nameof(ConstructorSingleAttribute.Value), RecordValue);
    }

    private bool RecordValue(object? value)
    {
        Value = value;
        ValueRecorded = true;

        return true;
    }
}
