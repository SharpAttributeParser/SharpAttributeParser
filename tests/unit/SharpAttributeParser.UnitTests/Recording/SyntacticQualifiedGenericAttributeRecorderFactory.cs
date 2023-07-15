namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SyntacticQualifiedGenericAttributeRecorderFactory : ISyntacticQualifiedGenericAttributeRecorderFactory
{
    private ISyntacticAttributeRecorderFactory Factory { get; }
    private ISyntacticAttributeMapper<ISyntacticQualifiedGenericAttributeRecordBuilder> ArgumentMapper { get; }

    public SyntacticQualifiedGenericAttributeRecorderFactory(ISyntacticAttributeRecorderFactory factory, ISyntacticAttributeMapper<ISyntacticQualifiedGenericAttributeRecordBuilder> argumentMapper)
    {
        Factory = factory;
        ArgumentMapper = argumentMapper;
    }

    ISyntacticAttributeRecorder<ISyntacticQualifiedGenericAttributeRecord> ISyntacticQualifiedGenericAttributeRecorderFactory.Create() => Factory.Create<ISyntacticQualifiedGenericAttributeRecord, ISyntacticQualifiedGenericAttributeRecordBuilder>(ArgumentMapper, new QualifiedGenericAttributeDataBuilder());

    private sealed class QualifiedGenericAttributeDataBuilder : ISyntacticQualifiedGenericAttributeRecordBuilder
    {
        private QualifiedGenericAttributeData Target { get; } = new();

        ISyntacticQualifiedGenericAttributeRecord IRecordBuilder<ISyntacticQualifiedGenericAttributeRecord>.Build() => Target;

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

        private sealed class QualifiedGenericAttributeData : ISyntacticQualifiedGenericAttributeRecord
        {
            public ExpressionSyntax? T1Syntax { get; set; }
            public bool T1SyntaxRecorded { get; set; }

            public ExpressionSyntax? T2Syntax { get; set; }
            public bool T2SyntaxRecorded { get; set; }
        }
    }
}
