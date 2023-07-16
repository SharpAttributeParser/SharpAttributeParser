namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis;

using SharpAttributeParser;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SemanticQualifiedGenericAttributeRecorderFactory : ISemanticQualifiedGenericAttributeRecorderFactory
{
    private ISemanticAttributeRecorderFactory Factory { get; }
    private ISemanticAttributeMapper<ISemanticQualifiedGenericAttributeRecordBuilder> ArgumentMapper { get; }

    public SemanticQualifiedGenericAttributeRecorderFactory(ISemanticAttributeRecorderFactory factory, ISemanticAttributeMapper<ISemanticQualifiedGenericAttributeRecordBuilder> argumentMapper)
    {
        Factory = factory;
        ArgumentMapper = argumentMapper;
    }

    ISemanticAttributeRecorder<ISemanticQualifiedGenericAttributeRecord> ISemanticQualifiedGenericAttributeRecorderFactory.Create() => Factory.Create<ISemanticQualifiedGenericAttributeRecord, ISemanticQualifiedGenericAttributeRecordBuilder>(ArgumentMapper, new QualifiedGenericAttributeRecordBuilder());

    private sealed class QualifiedGenericAttributeRecordBuilder : ISemanticQualifiedGenericAttributeRecordBuilder
    {
        private QualifiedGenericAttributeRecord Target { get; } = new();

        ISemanticQualifiedGenericAttributeRecord IRecordBuilder<ISemanticQualifiedGenericAttributeRecord>.Build() => Target;

        void ISemanticQualifiedGenericAttributeRecordBuilder.WithT1(ITypeSymbol t1)
        {
            Target.T1 = t1;
            Target.T1Recorded = true;
        }

        void ISemanticQualifiedGenericAttributeRecordBuilder.WithT2(ITypeSymbol t2)
        {
            Target.T2 = t2;
            Target.T2Recorded = true;
        }

        private sealed class QualifiedGenericAttributeRecord : ISemanticQualifiedGenericAttributeRecord
        {
            public ITypeSymbol T1 { get; set; } = null!;
            public bool T1Recorded { get; set; }

            public ITypeSymbol T2 { get; set; } = null!;
            public bool T2Recorded { get; set; }
        }
    }
}
