namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SemanticCombinedAttributeRecorderFactory : ISemanticCombinedAttributeRecorderFactory
{
    private ISemanticAttributeRecorderFactory Factory { get; }
    private ISemanticAttributeMapper<ICombinedAttributeDataBuilder> ArgumentMapper { get; }

    public SemanticCombinedAttributeRecorderFactory(ISemanticAttributeRecorderFactory factory, ISemanticAttributeMapper<ICombinedAttributeDataBuilder> argumentMapper)
    {
        Factory = factory;
        ArgumentMapper = argumentMapper;
    }

    ISemanticAttributeRecorder<ICombinedAttributeData> ISemanticCombinedAttributeRecorderFactory.Create() => Factory.Create<ICombinedAttributeData, ICombinedAttributeDataBuilder>(ArgumentMapper, new CombinedAttributeDataBuilder());

    private sealed class CombinedAttributeDataBuilder : ICombinedAttributeDataBuilder
    {
        private CombinedAttributeData Target { get; } = new();

        ICombinedAttributeData IAttributeDataBuilder<ICombinedAttributeData>.Build() => Target;

        void ICombinedAttributeDataBuilder.WithT1(ITypeSymbol t1, Location location)
        {
            Target.T1 = t1;
            Target.T1Recorded = true;
            Target.T1Location = location;
        }

        void ICombinedAttributeDataBuilder.WithT2(ITypeSymbol t2, Location location)
        {
            Target.T2 = t2;
            Target.T2Recorded = true;
            Target.T2Location = location;
        }

        void ICombinedAttributeDataBuilder.WithSimpleValue(object? value, Location location)
        {
            Target.SimpleValue = value;
            Target.SimpleValueRecorded = true;
            Target.SimpleValueLocation = location;
        }

        void ICombinedAttributeDataBuilder.WithArrayValue(IReadOnlyList<object?>? value, CollectionLocation location)
        {
            Target.ArrayValue = value;
            Target.ArrayValueRecorded = true;
            Target.ArrayValueLocation = location;
        }

        void ICombinedAttributeDataBuilder.WithParamsValue(IReadOnlyList<object?>? value, CollectionLocation location)
        {
            Target.ParamsValue = value;
            Target.ParamsValueRecorded = true;
            Target.ParamsValueLocation = location;
        }

        void ICombinedAttributeDataBuilder.WithSimpleNamedValue(object? value, Location location)
        {
            Target.SimpleNamedValue = value;
            Target.SimpleNamedValueRecorded = true;
            Target.SimpleNamedValueLocation = location;
        }

        void ICombinedAttributeDataBuilder.WithArrayNamedValue(IReadOnlyList<object?>? value, CollectionLocation location)
        {
            Target.ArrayNamedValue = value;
            Target.ArrayNamedValueRecorded = true;
            Target.ArrayNamedValueLocation = location;
        }

        private sealed class CombinedAttributeData : ICombinedAttributeData
        {
            public ITypeSymbol? T1 { get; set; }
            public bool T1Recorded { get; set; }
            public Location T1Location { get; set; } = Location.None;

            public ITypeSymbol? T2 { get; set; }
            public bool T2Recorded { get; set; }
            public Location T2Location { get; set; } = Location.None;

            public object? SimpleValue { get; set; }
            public bool SimpleValueRecorded { get; set; }
            public Location SimpleValueLocation { get; set; } = Location.None;

            public IReadOnlyList<object?>? ArrayValue { get; set; }
            public bool ArrayValueRecorded { get; set; }
            public CollectionLocation ArrayValueLocation { get; set; } = CollectionLocation.None;

            public IReadOnlyList<object?>? ParamsValue { get; set; }
            public bool ParamsValueRecorded { get; set; }
            public CollectionLocation ParamsValueLocation { get; set; } = CollectionLocation.None;

            public object? SimpleNamedValue { get; set; }
            public bool SimpleNamedValueRecorded { get; set; }
            public Location SimpleNamedValueLocation { get; set; } = Location.None;

            public IReadOnlyList<object?>? ArrayNamedValue { get; set; }
            public bool ArrayNamedValueRecorded { get; set; }
            public CollectionLocation ArrayNamedValueLocation { get; set; } = CollectionLocation.None;
        }
    }
}
