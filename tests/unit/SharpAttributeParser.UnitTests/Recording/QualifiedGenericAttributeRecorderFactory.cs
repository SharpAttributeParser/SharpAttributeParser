namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class QualifiedGenericAttributeRecorderFactory : IQualifiedGenericAttributeRecorderFactory
{
    private IAttributeRecorderFactory Factory { get; }
    private IAttributeMapper<IQualifiedGenericAttributeRecordBuilder> ArgumentMapper { get; }

    public QualifiedGenericAttributeRecorderFactory(IAttributeRecorderFactory factory, IAttributeMapper<IQualifiedGenericAttributeRecordBuilder> argumentMapper)
    {
        Factory = factory;
        ArgumentMapper = argumentMapper;
    }

    IAttributeRecorder<IQualifiedGenericAttributeRecord> IQualifiedGenericAttributeRecorderFactory.Create() => Factory.Create<IQualifiedGenericAttributeRecord, IQualifiedGenericAttributeRecordBuilder>(ArgumentMapper, new QualifiedGenericAttributeDataBuilder());

    private sealed class QualifiedGenericAttributeDataBuilder : IQualifiedGenericAttributeRecordBuilder
    {
        private QualifiedGenericAttributeData Target { get; } = new();

        IQualifiedGenericAttributeRecord IRecordBuilder<IQualifiedGenericAttributeRecord>.Build() => Target;
        ISemanticQualifiedGenericAttributeRecord IRecordBuilder<ISemanticQualifiedGenericAttributeRecord>.Build() => Target;
        ISyntacticQualifiedGenericAttributeRecord IRecordBuilder<ISyntacticQualifiedGenericAttributeRecord>.Build() => Target;

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

        void ISyntacticQualifiedGenericAttributeRecordBuilder.WithT1Syntax(ExpressionSyntax syntax)
        {
            Target.T1Syntax = syntax;
            Target.T1SyntaxRecorded = true;
        }

        void ISyntacticQualifiedGenericAttributeRecordBuilder.WithT2Syntax(ExpressionSyntax syntax)
        {
            Target.T2Syntax = syntax;
            Target.T2SyntaxRecorded = true;
        }

        private sealed class QualifiedGenericAttributeData : IQualifiedGenericAttributeRecord
        {
            public ITypeSymbol T1 { get; set; } = null!;
            public bool T1Recorded { get; set; }

            public ITypeSymbol T2 { get; set; } = null!;
            public bool T2Recorded { get; set; }

            public ExpressionSyntax? T1Syntax { get; set; }
            public bool T1SyntaxRecorded { get; set; }

            public ExpressionSyntax? T2Syntax { get; set; }
            public bool T2SyntaxRecorded { get; set; }
        }
    }
}
