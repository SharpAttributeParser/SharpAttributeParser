namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SyntacticSimpleConstructorAttributeRecorderFactory : ISyntacticSimpleConstructorAttributeRecorderFactory
{
    private ISyntacticAttributeRecorderFactory Factory { get; }
    private ISyntacticAttributeMapper<ISyntacticSimpleConstructorAttributeRecordBuilder> ArgumentMapper { get; }

    public SyntacticSimpleConstructorAttributeRecorderFactory(ISyntacticAttributeRecorderFactory factory, ISyntacticAttributeMapper<ISyntacticSimpleConstructorAttributeRecordBuilder> argumentMapper)
    {
        Factory = factory;
        ArgumentMapper = argumentMapper;
    }

    ISyntacticAttributeRecorder<ISyntacticSimpleConstructorAttributeRecord> ISyntacticSimpleConstructorAttributeRecorderFactory.Create() => Factory.Create<ISyntacticSimpleConstructorAttributeRecord, ISyntacticSimpleConstructorAttributeRecordBuilder>(ArgumentMapper, new SimpleConstructorAttributeDataBuilder());

    private sealed class SimpleConstructorAttributeDataBuilder : ISyntacticSimpleConstructorAttributeRecordBuilder
    {
        private SimpleConstructorAttributeData Target { get; } = new();

        ISyntacticSimpleConstructorAttributeRecord IRecordBuilder<ISyntacticSimpleConstructorAttributeRecord>.Build() => Target;

        void ISyntacticSimpleConstructorAttributeRecordBuilder.WithValueSyntax(ExpressionSyntax syntax)
        {
            Target.ValueSyntax = syntax;
            Target.ValueSyntaxRecorded = true;
        }

        private sealed class SimpleConstructorAttributeData : ISyntacticSimpleConstructorAttributeRecord
        {
            public ExpressionSyntax? ValueSyntax { get; set; }
            public bool ValueSyntaxRecorded { get; set; }
        }
    }
}
