namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SyntacticArrayConstructorAttributeRecorderFactory : ISyntacticArrayConstructorAttributeRecorderFactory
{
    private ISyntacticAttributeRecorderFactory Factory { get; }
    private ISyntacticAttributeMapper<ISyntacticArrayConstructorAttributeRecordBuilder> ArgumentMapper { get; }

    public SyntacticArrayConstructorAttributeRecorderFactory(ISyntacticAttributeRecorderFactory factory, ISyntacticAttributeMapper<ISyntacticArrayConstructorAttributeRecordBuilder> argumentMapper)
    {
        Factory = factory;
        ArgumentMapper = argumentMapper;
    }

    ISyntacticAttributeRecorder<ISyntacticArrayConstructorAttributeRecord> ISyntacticArrayConstructorAttributeRecorderFactory.Create() => Factory.Create<ISyntacticArrayConstructorAttributeRecord, ISyntacticArrayConstructorAttributeRecordBuilder>(ArgumentMapper, new ArrayConstructorAttributeDataBuilder());

    private sealed class ArrayConstructorAttributeDataBuilder : ISyntacticArrayConstructorAttributeRecordBuilder
    {
        private ArrayConstructorAttributeData Target { get; } = new();

        ISyntacticArrayConstructorAttributeRecord IRecordBuilder<ISyntacticArrayConstructorAttributeRecord>.Build() => Target;

        void ISyntacticArrayConstructorAttributeRecordBuilder.WithValueSyntax(ExpressionSyntax syntax)
        {
            Target.ValueSyntax = syntax;
            Target.ValueSyntaxRecorded = true;
        }

        private sealed class ArrayConstructorAttributeData : ISyntacticArrayConstructorAttributeRecord
        {
            public ExpressionSyntax? ValueSyntax { get; set; }
            public bool ValueSyntaxRecorded { get; set; }
        }
    }
}
