namespace SharpAttributeParser.SyntacticAttributeParserCases;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;

public sealed class SyntacticNamedAttributeRecorder : ASyntacticArgumentRecorder
{
    public object? SingleValue { get; private set; }
    public bool SingleValueRecorded { get; private set; }
    public Location? SingleValueLocation { get; private set; }

    public IReadOnlyList<object?>? ArrayValue { get; private set; }
    public bool ArrayValueRecorded { get; private set; }
    public CollectionLocation? ArrayValueLocation { get; private set; }

    protected override IEnumerable<(string, DSyntacticSingleRecorder)> AddSingleRecorders()
    {
        yield return (nameof(NamedAttribute.SingleValue), RecordSingleValue);
    }

    protected override IEnumerable<(string, DSyntacticArrayRecorder)> AddArrayRecorders()
    {
        yield return (nameof(NamedAttribute.ArrayValue), RecordArrayValue);
    }

    private bool RecordSingleValue(object? value, Location location)
    {
        SingleValue = value;
        SingleValueRecorded = true;
        SingleValueLocation = location;

        return true;
    }

    private bool RecordArrayValue(IReadOnlyList<object?>? value, CollectionLocation location)
    {
        ArrayValue = value;
        ArrayValueRecorded = true;
        ArrayValueLocation = location;

        return true;
    }
}
