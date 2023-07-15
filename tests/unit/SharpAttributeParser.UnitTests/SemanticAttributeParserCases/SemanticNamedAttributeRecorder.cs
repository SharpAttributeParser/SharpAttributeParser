namespace SharpAttributeParser.SemanticAttributeParserCases;

using System.Collections.Generic;

public sealed class SemanticNamedAttributeRecorder : ASemanticArgumentRecorder
{
    public object? SingleValue { get; private set; }
    public bool SingleValueRecorded { get; private set; }

    public IReadOnlyList<object?>? ArrayValue { get; private set; }
    public bool ArrayValueRecorded { get; private set; }

    protected override IEnumerable<(string, DSemanticSingleRecorder)> AddSingleRecorders()
    {
        yield return (nameof(NamedAttribute.SingleValue), RecordSingleValue);
    }

    protected override IEnumerable<(string, DSemanticArrayRecorder)> AddArrayRecorders()
    {
        yield return (nameof(NamedAttribute.ArrayValue), RecordArrayValue);
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
