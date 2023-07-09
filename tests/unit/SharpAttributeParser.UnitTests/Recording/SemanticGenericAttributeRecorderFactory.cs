namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis;

using SharpAttributeParser;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SemanticGenericAttributeRecorderFactory : ISemanticGenericAttributeRecorderFactory
{
    private ISemanticAttributeRecorderFactory Factory { get; }
    private ISemanticAttributeMapper<ISemanticGenericAttributeRecordBuilder> ArgumentMapper { get; }

    public SemanticGenericAttributeRecorderFactory(ISemanticAttributeRecorderFactory factory, ISemanticAttributeMapper<ISemanticGenericAttributeRecordBuilder> argumentMapper)
    {
        Factory = factory;
        ArgumentMapper = argumentMapper;
    }

    ISemanticAttributeRecorder<ISemanticGenericAttributeRecord> ISemanticGenericAttributeRecorderFactory.Create() => Factory.Create<ISemanticGenericAttributeRecord, ISemanticGenericAttributeRecordBuilder>(ArgumentMapper, new GenericAttributeDataBuilder());

    private sealed class GenericAttributeDataBuilder : ISemanticGenericAttributeRecordBuilder
    {
        private GenericAttributeData Target { get; } = new();

        ISemanticGenericAttributeRecord IRecordBuilder<ISemanticGenericAttributeRecord>.Build() => Target;

        void ISemanticGenericAttributeRecordBuilder.WithT1(ITypeSymbol t1)
        {
            Target.T1 = t1;
            Target.T1Recorded = true;
        }

        void ISemanticGenericAttributeRecordBuilder.WithT2(ITypeSymbol t2)
        {
            Target.T2 = t2;
            Target.T2Recorded = true;
        }

        private sealed class GenericAttributeData : ISemanticGenericAttributeRecord
        {
            public ITypeSymbol? T1 { get; set; }
            public bool T1Recorded { get; set; }

            public ITypeSymbol? T2 { get; set; }
            public bool T2Recorded { get; set; }
        }
    }
}
