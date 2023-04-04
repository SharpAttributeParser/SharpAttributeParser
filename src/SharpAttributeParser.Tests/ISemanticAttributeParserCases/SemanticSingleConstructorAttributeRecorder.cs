namespace SharpAttributeParser.Tests.ISemanticAttributeParserCases;

using System.Collections.Generic;

public sealed class SemanticSingleConstructorAttributeRecorder : ASemanticArgumentRecorder
{
    public object? Value { get; private set; }
    public bool ValueRecorded { get; private set; }

    protected override IEnumerable<(string, DSingleRecorder)> AddSingleRecorders()
    {
        yield return ("Value", RecordValue);
    }

    private bool RecordValue(object? value)
    {
        Value = value;
        ValueRecorded = true;

        return true;
    }
}
