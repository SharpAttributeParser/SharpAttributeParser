namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class GenericAttributeRecorderFactory : IGenericAttributeRecorderFactory
{
    private IAttributeRecorderFactory Factory { get; }
    private IAttributeMapper<IGenericAttributeRecordBuilder> ArgumentMapper { get; }

    public GenericAttributeRecorderFactory(IAttributeRecorderFactory factory, IAttributeMapper<IGenericAttributeRecordBuilder> argumentMapper)
    {
        Factory = factory;
        ArgumentMapper = argumentMapper;
    }

    IAttributeRecorder<IGenericAttributeRecord> IGenericAttributeRecorderFactory.Create() => Factory.Create<IGenericAttributeRecord, IGenericAttributeRecordBuilder>(ArgumentMapper, new GenericAttributeDataBuilder());

    private sealed class GenericAttributeDataBuilder : IGenericAttributeRecordBuilder
    {
        private GenericAttributeData Target { get; } = new();

        IGenericAttributeRecord IRecordBuilder<IGenericAttributeRecord>.Build() => Target;
        ISemanticGenericAttributeRecord IRecordBuilder<ISemanticGenericAttributeRecord>.Build() => Target;
        ISyntacticGenericAttributeRecord IRecordBuilder<ISyntacticGenericAttributeRecord>.Build() => Target;

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

        private sealed class GenericAttributeData : IGenericAttributeRecord
        {
            public ITypeSymbol? T1 { get; set; }
            public bool T1Recorded { get; set; }

            public ITypeSymbol? T2 { get; set; }
            public bool T2Recorded { get; set; }

            public ExpressionSyntax? T1Syntax { get; set; }
            public bool T1SyntaxRecorded { get; set; }

            public ExpressionSyntax? T2Syntax { get; set; }
            public bool T2SyntaxRecorded { get; set; }
        }
    }
}
