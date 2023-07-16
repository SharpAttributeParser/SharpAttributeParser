namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SyntacticNamedAttributeRecorderFactory : ISyntacticNamedAttributeRecorderFactory
{
    private ISyntacticAttributeRecorderFactory Factory { get; }
    private ISyntacticAttributeMapper<ISyntacticNamedAttributeRecordBuilder> ArgumentMapper { get; }

    public SyntacticNamedAttributeRecorderFactory(ISyntacticAttributeRecorderFactory factory, ISyntacticAttributeMapper<ISyntacticNamedAttributeRecordBuilder> argumentMapper)
    {
        Factory = factory;
        ArgumentMapper = argumentMapper;
    }

    ISyntacticAttributeRecorder<ISyntacticNamedAttributeRecord> ISyntacticNamedAttributeRecorderFactory.Create() => Factory.Create<ISyntacticNamedAttributeRecord, ISyntacticNamedAttributeRecordBuilder>(ArgumentMapper, new NamedAttributeRecordBuilder());

    private sealed class NamedAttributeRecordBuilder : ISyntacticNamedAttributeRecordBuilder
    {
        private NamedAttributeRecord Target { get; } = new();

        ISyntacticNamedAttributeRecord IRecordBuilder<ISyntacticNamedAttributeRecord>.Build() => Target;

        void ISyntacticNamedAttributeRecordBuilder.WithSimpleValueSyntax(ExpressionSyntax syntax)
        {
            Target.SimpleValueSyntax = syntax;
            Target.SimpleValueSyntaxRecorded = true;
        }

        void ISyntacticNamedAttributeRecordBuilder.WithArrayValueSyntax(ExpressionSyntax syntax)
        {
            Target.ArrayValueSyntax = syntax;
            Target.ArrayValueSyntaxRecorded = true;
        }

        private sealed class NamedAttributeRecord : ISyntacticNamedAttributeRecord
        {
            public ExpressionSyntax? SimpleValueSyntax { get; set; }
            public bool SimpleValueSyntaxRecorded { get; set; }

            public ExpressionSyntax? ArrayValueSyntax { get; set; }
            public bool ArrayValueSyntaxRecorded { get; set; }
        }
    }
}
