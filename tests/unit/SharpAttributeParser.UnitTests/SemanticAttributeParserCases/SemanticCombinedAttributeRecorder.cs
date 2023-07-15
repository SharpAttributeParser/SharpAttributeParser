namespace SharpAttributeParser.SemanticAttributeParserCases;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;

public sealed class SemanticCombinedAttributeRecorder : ASemanticArgumentRecorder
{
    public ITypeSymbol? T1 { get; private set; }
    public bool T1Recorded { get; private set; }

    public ITypeSymbol? T2 { get; private set; }
    public bool T2Recorded { get; private set; }

    public object? SingleValue { get; private set; }
    public bool SingleValueRecorded { get; private set; }

    public IReadOnlyList<object?>? ArrayValue { get; private set; }
    public bool ArrayValueRecorded { get; private set; }

    public IReadOnlyList<object?>? ParamsValue { get; private set; }
    public bool ParamsValueRecorded { get; private set; }

    public object? NamedSingleValue { get; private set; }
    public bool NamedSingleValueRecorded { get; private set; }

    public IReadOnlyList<object?>? NamedArrayValue { get; private set; }
    public bool NamedArrayValueRecorded { get; private set; }

    protected override IEnumerable<(int, DSemanticGenericRecorder)> AddIndexedGenericRecorders()
    {
        yield return (0, RecordT1);
        yield return (1, RecordT2);
    }

    protected override IEnumerable<(string, DSemanticSingleRecorder)> AddSingleRecorders()
    {
        yield return (nameof(CombinedAttribute<object, object>.SingleValue), RecordSingleValue);
        yield return (nameof(CombinedAttribute<object, object>.NamedSingleValue), RecordNamedSingleValue);
    }

    protected override IEnumerable<(string, DSemanticArrayRecorder)> AddArrayRecorders()
    {
        yield return (nameof(CombinedAttribute<object, object>.ArrayValue), RecordArrayValue);
        yield return (nameof(CombinedAttribute<object, object>.ParamsValue), RecordParamsValue);
        yield return (nameof(CombinedAttribute<object, object>.NamedArrayValue), RecordNamedArrayValue);
    }

    private bool RecordT1(ITypeSymbol value)
    {
        T1 = value;
        T1Recorded = true;

        return true;
    }

    private bool RecordT2(ITypeSymbol value)
    {
        T2 = value;
        T2Recorded = true;

        return true;
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

    private bool RecordParamsValue(IReadOnlyList<object?>? value)
    {
        ParamsValue = value;
        ParamsValueRecorded = true;

        return true;
    }

    private bool RecordNamedSingleValue(object? value)
    {
        NamedSingleValue = value;
        NamedSingleValueRecorded = true;

        return true;
    }

    private bool RecordNamedArrayValue(IReadOnlyList<object?>? value)
    {
        NamedArrayValue = value;
        NamedArrayValueRecorded = true;

        return true;
    }
}
