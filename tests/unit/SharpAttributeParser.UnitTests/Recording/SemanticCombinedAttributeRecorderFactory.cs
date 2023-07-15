namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SemanticCombinedAttributeRecorderFactory : ISemanticCombinedAttributeRecorderFactory
{
    private ISemanticAttributeRecorderFactory Factory { get; }
    private ISemanticAttributeMapper<ISemanticCombinedAttributeRecordBuilder> ArgumentMapper { get; }

    public SemanticCombinedAttributeRecorderFactory(ISemanticAttributeRecorderFactory factory, ISemanticAttributeMapper<ISemanticCombinedAttributeRecordBuilder> argumentMapper)
    {
        Factory = factory;
        ArgumentMapper = argumentMapper;
    }

    ISemanticAttributeRecorder<ISemanticCombinedAttributeRecord> ISemanticCombinedAttributeRecorderFactory.Create() => Factory.Create<ISemanticCombinedAttributeRecord, ISemanticCombinedAttributeRecordBuilder>(ArgumentMapper, new CombinedAttributeDataBuilder());

    private sealed class CombinedAttributeDataBuilder : ISemanticCombinedAttributeRecordBuilder
    {
        private CombinedAttributeData Target { get; } = new();

        ISemanticCombinedAttributeRecord IRecordBuilder<ISemanticCombinedAttributeRecord>.Build() => Target;

        void ISemanticCombinedAttributeRecordBuilder.WithT1(ITypeSymbol t1)
        {
            Target.T1 = t1;
            Target.T1Recorded = true;
        }

        void ISemanticCombinedAttributeRecordBuilder.WithT2(ITypeSymbol t2)
        {
            Target.T2 = t2;
            Target.T2Recorded = true;
        }

        void ISemanticCombinedAttributeRecordBuilder.WithSimpleValue(object? value)
        {
            Target.SimpleValue = value;
            Target.SimpleValueRecorded = true;
        }

        void ISemanticCombinedAttributeRecordBuilder.WithArrayValue(IReadOnlyList<object?>? value)
        {
            Target.ArrayValue = value;
            Target.ArrayValueRecorded = true;
        }

        void ISemanticCombinedAttributeRecordBuilder.WithParamsValue(IReadOnlyList<object?>? value)
        {
            Target.ParamsValue = value;
            Target.ParamsValueRecorded = true;
        }

        void ISemanticCombinedAttributeRecordBuilder.WithSimpleNamedValue(object? value)
        {
            Target.SimpleNamedValue = value;
            Target.SimpleNamedValueRecorded = true;
        }

        void ISemanticCombinedAttributeRecordBuilder.WithArrayNamedValue(IReadOnlyList<object?>? value)
        {
            Target.ArrayNamedValue = value;
            Target.ArrayNamedValueRecorded = true;
        }

        private sealed class CombinedAttributeData : ISemanticCombinedAttributeRecord
        {
            public ITypeSymbol? T1 { get; set; }
            public bool T1Recorded { get; set; }

            public ITypeSymbol? T2 { get; set; }
            public bool T2Recorded { get; set; }

            public object? SimpleValue { get; set; }
            public bool SimpleValueRecorded { get; set; }

            public IReadOnlyList<object?>? ArrayValue { get; set; }
            public bool ArrayValueRecorded { get; set; }

            public IReadOnlyList<object?>? ParamsValue { get; set; }
            public bool ParamsValueRecorded { get; set; }

            public object? SimpleNamedValue { get; set; }
            public bool SimpleNamedValueRecorded { get; set; }

            public IReadOnlyList<object?>? ArrayNamedValue { get; set; }
            public bool ArrayNamedValueRecorded { get; set; }
        }
    }
}
