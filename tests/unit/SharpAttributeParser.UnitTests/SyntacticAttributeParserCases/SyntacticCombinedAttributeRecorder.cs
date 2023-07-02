namespace SharpAttributeParser.SyntacticAttributeParserCases;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;

public sealed class SyntacticCombinedAttributeRecorder : ASyntacticArgumentRecorder
{
    public ITypeSymbol? T1 { get; private set; }
    public bool T1Recorded { get; private set; }
    public Location? T1Location { get; private set; }

    public ITypeSymbol? T2 { get; private set; }
    public bool T2Recorded { get; private set; }
    public Location? T2Location { get; private set; }

    public object? SingleValue { get; private set; }
    public bool SingleValueRecorded { get; private set; }
    public Location? SingleValueLocation { get; private set; }

    public IReadOnlyList<object?>? ArrayValue { get; private set; }
    public bool ArrayValueRecorded { get; private set; }
    public CollectionLocation? ArrayValueLocation { get; private set; }

    public IReadOnlyList<object?>? ParamsValue { get; private set; }
    public bool ParamsValueRecorded { get; private set; }
    public CollectionLocation? ParamsValueLocation { get; private set; }

    public object? NamedSingleValue { get; private set; }
    public bool NamedSingleValueRecorded { get; private set; }
    public Location? NamedSingleValueLocation { get; private set; }

    public IReadOnlyList<object?>? NamedArrayValue { get; private set; }
    public bool NamedArrayValueRecorded { get; private set; }
    public CollectionLocation? NamedArrayValueLocation { get; private set; }

    protected override IEnumerable<(int, DSyntacticGenericRecorder)> AddIndexedGenericRecorders()
    {
        yield return (0, RecordT1);
        yield return (1, RecordT2);
    }

    protected override IEnumerable<(string, DSyntacticSingleRecorder)> AddSingleRecorders()
    {
        yield return (nameof(CombinedAttribute<object, object>.SingleValue), RecordValue);
        yield return (nameof(CombinedAttribute<object, object>.NamedSingleValue), RecordNamedSingleValue);
    }

    protected override IEnumerable<(string, DSyntacticArrayRecorder)> AddArrayRecorders()
    {
        yield return (nameof(CombinedAttribute<object, object>.ArrayValue), RecordArrayValue);
        yield return (nameof(CombinedAttribute<object, object>.ParamsValue), RecordParamsValue);
        yield return (nameof(CombinedAttribute<object, object>.NamedArrayValue), RecordNamedArrayValue);
    }

    private bool RecordT1(ITypeSymbol value, Location location)
    {
        T1 = value;
        T1Recorded = true;
        T1Location = location;

        return true;
    }

    private bool RecordT2(ITypeSymbol value, Location location)
    {
        T2 = value;
        T2Recorded = true;
        T2Location = location;

        return true;
    }

    private bool RecordValue(object? value, Location location)
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

    private bool RecordParamsValue(IReadOnlyList<object?>? value, CollectionLocation location)
    {
        ParamsValue = value;
        ParamsValueRecorded = true;
        ParamsValueLocation = location;

        return true;
    }

    private bool RecordNamedSingleValue(object? value, Location location)
    {
        NamedSingleValue = value;
        NamedSingleValueRecorded = true;
        NamedSingleValueLocation = location;

        return true;
    }

    private bool RecordNamedArrayValue(IReadOnlyList<object?>? value, CollectionLocation location)
    {
        NamedArrayValue = value;
        NamedArrayValueRecorded = true;
        NamedArrayValueLocation = location;

        return true;
    }
}
