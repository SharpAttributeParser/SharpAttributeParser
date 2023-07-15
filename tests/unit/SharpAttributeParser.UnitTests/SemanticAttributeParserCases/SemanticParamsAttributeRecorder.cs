namespace SharpAttributeParser.SemanticAttributeParserCases;

using SharpAttributeParser;

using System.Collections.Generic;

public sealed class SemanticParamsAttributeRecorder : ASemanticArgumentRecorder
{
    public IReadOnlyList<object?>? Value { get; private set; }
    public bool ValueRecorded { get; private set; }

    protected override IEnumerable<(string, DSemanticArrayRecorder)> AddArrayRecorders()
    {
        yield return (nameof(ParamsAttribute.Value), RecordValue);
    }

    private bool RecordValue(IReadOnlyList<object?>? value)
    {
        Value = value;
        ValueRecorded = true;

        return true;
    }
}
