namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SyntacticGenericAttributeRecorderFactory : ISyntacticGenericAttributeRecorderFactory
{
    private ISyntacticAttributeRecorderFactory Factory { get; }
    private ISyntacticAttributeMapper<ISyntacticGenericAttributeRecordBuilder> ArgumentMapper { get; }

    public SyntacticGenericAttributeRecorderFactory(ISyntacticAttributeRecorderFactory factory, ISyntacticAttributeMapper<ISyntacticGenericAttributeRecordBuilder> argumentMapper)
    {
        Factory = factory;
        ArgumentMapper = argumentMapper;
    }

    ISyntacticAttributeRecorder<ISyntacticGenericAttributeRecord> ISyntacticGenericAttributeRecorderFactory.Create() => Factory.Create<ISyntacticGenericAttributeRecord, ISyntacticGenericAttributeRecordBuilder>(ArgumentMapper, new GenericAttributeRecordBuilder());

    private sealed class GenericAttributeRecordBuilder : ISyntacticGenericAttributeRecordBuilder
    {
        private GenericAttributeRecord Target { get; } = new();

        ISyntacticGenericAttributeRecord IRecordBuilder<ISyntacticGenericAttributeRecord>.Build() => Target;

        void ISyntacticGenericAttributeRecordBuilder.WithT1Syntax(ExpressionSyntax syntax)
        {
            Target.T1Syntax = syntax;
            Target.T1SyntaxRecorded = true;
        }

        void ISyntacticGenericAttributeRecordBuilder.WithT2Syntax(ExpressionSyntax syntax)
        {
            Target.T2Syntax = syntax;
            Target.T2SyntaxRecorded = true;
        }

        private sealed class GenericAttributeRecord : ISyntacticGenericAttributeRecord
        {
            public ExpressionSyntax? T1Syntax { get; set; }
            public bool T1SyntaxRecorded { get; set; }

            public ExpressionSyntax? T2Syntax { get; set; }
            public bool T2SyntaxRecorded { get; set; }
        }
    }
}
