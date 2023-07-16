namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SyntacticParamsAttributeRecorderFactory : ISyntacticParamsAttributeRecorderFactory
{
    private ISyntacticAttributeRecorderFactory Factory { get; }
    private ISyntacticAttributeMapper<ISyntacticParamsAttributeRecordBuilder> ArgumentMapper { get; }

    public SyntacticParamsAttributeRecorderFactory(ISyntacticAttributeRecorderFactory factory, ISyntacticAttributeMapper<ISyntacticParamsAttributeRecordBuilder> argumentMapper)
    {
        Factory = factory;
        ArgumentMapper = argumentMapper;
    }

    ISyntacticAttributeRecorder<ISyntacticParamsAttributeRecord> ISyntacticParamsAttributeRecorderFactory.Create() => Factory.Create<ISyntacticParamsAttributeRecord, ISyntacticParamsAttributeRecordBuilder>(ArgumentMapper, new ParamsAttributeRecordBuilder());

    private sealed class ParamsAttributeRecordBuilder : ISyntacticParamsAttributeRecordBuilder
    {
        private ParamsAttributeRecord Target { get; } = new();

        ISyntacticParamsAttributeRecord IRecordBuilder<ISyntacticParamsAttributeRecord>.Build() => Target;

        void ISyntacticParamsAttributeRecordBuilder.WithValueSyntax(OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
        {
            Target.ValueSyntax = syntax;
            Target.ValueSyntaxRecorded = true;
        }

        private sealed class ParamsAttributeRecord : ISyntacticParamsAttributeRecord
        {
            public OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> ValueSyntax { get; set; }
            public bool ValueSyntaxRecorded { get; set; }
        }
    }
}
