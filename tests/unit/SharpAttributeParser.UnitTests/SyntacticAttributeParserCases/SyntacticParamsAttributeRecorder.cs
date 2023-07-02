namespace SharpAttributeParser.SyntacticAttributeParserCases;

using SharpAttributeParser;

using System.Collections.Generic;

public sealed class SyntacticParamsAttributeRecorder : ASyntacticArgumentRecorder
{
    public IReadOnlyList<object?>? Value { get; private set; }
    public bool ValueRecorded { get; private set; }
    public CollectionLocation? ValueLocation { get; private set; }

    protected override IEnumerable<(string, DSyntacticArrayRecorder)> AddArrayRecorders()
    {
        yield return (nameof(ParamsAttribute.Value), RecordValue);
    }

    private bool RecordValue(IReadOnlyList<object?>? value, CollectionLocation location)
    {
        Value = value;
        ValueRecorded = true;
        ValueLocation = location;

        return true;
    }
}
