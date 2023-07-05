namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis;

using SharpAttributeParser;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SemanticQualifiedGenericAttributeRecorderFactory : ISemanticQualifiedGenericAttributeRecorderFactory
{
    private ISemanticAttributeRecorderFactory Factory { get; }
    private ISemanticAttributeMapper<IQualifiedGenericAttributeDataBuilder> ArgumentMapper { get; }

    public SemanticQualifiedGenericAttributeRecorderFactory(ISemanticAttributeRecorderFactory factory, ISemanticAttributeMapper<IQualifiedGenericAttributeDataBuilder> argumentMapper)
    {
        Factory = factory;
        ArgumentMapper = argumentMapper;
    }

    ISemanticAttributeRecorder<IQualifiedGenericAttributeData> ISemanticQualifiedGenericAttributeRecorderFactory.Create() => Factory.Create<IQualifiedGenericAttributeData, IQualifiedGenericAttributeDataBuilder>(ArgumentMapper, new QualifiedGenericAttributeDataBuilder());

    private sealed class QualifiedGenericAttributeDataBuilder : IQualifiedGenericAttributeDataBuilder
    {
        private QualifiedGenericAttributeData Target { get; } = new();

        IQualifiedGenericAttributeData IAttributeDataBuilder<IQualifiedGenericAttributeData>.Build() => Target;

        void IQualifiedGenericAttributeDataBuilder.WithT1(ITypeSymbol t1, Location t1Location)
        {
            Target.T1 = t1;
            Target.T1Recorded = true;
            Target.T1Location = t1Location;
        }

        void IQualifiedGenericAttributeDataBuilder.WithT2(ITypeSymbol t2, Location t2Location)
        {
            Target.T2 = t2;
            Target.T2Recorded = true;
            Target.T2Location = t2Location;
        }

        private sealed class QualifiedGenericAttributeData : IQualifiedGenericAttributeData
        {
            public ITypeSymbol T1 { get; set; } = null!;
            public Location T1Location { get; set; } = null!;
            public bool T1Recorded { get; set; }

            public ITypeSymbol T2 { get; set; } = null!;
            public Location T2Location { get; set; } = null!;
            public bool T2Recorded { get; set; }
        }
    }
}
