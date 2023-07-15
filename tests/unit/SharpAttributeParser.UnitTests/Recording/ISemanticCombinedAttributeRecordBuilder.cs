namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;

internal interface ISemanticCombinedAttributeRecordBuilder : IRecordBuilder<ISemanticCombinedAttributeRecord>
{
    public abstract void WithT1(ITypeSymbol t1);
    public abstract void WithT2(ITypeSymbol t2);

    public abstract void WithSimpleValue(object? value);
    public abstract void WithArrayValue(IReadOnlyList<object?>? value);
    public abstract void WithParamsValue(IReadOnlyList<object?>? value);

    public abstract void WithSimpleNamedValue(object? value);
    public abstract void WithArrayNamedValue(IReadOnlyList<object?>? value);
}
