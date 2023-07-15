namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SyntacticCombinedAttributeRecorderFactory : ISyntacticCombinedAttributeRecorderFactory
{
    private ISyntacticAttributeRecorderFactory Factory { get; }
    private ISyntacticAttributeMapper<ISyntacticCombinedAttributeRecordBuilder> ArgumentMapper { get; }

    public SyntacticCombinedAttributeRecorderFactory(ISyntacticAttributeRecorderFactory factory, ISyntacticAttributeMapper<ISyntacticCombinedAttributeRecordBuilder> argumentMapper)
    {
        Factory = factory;
        ArgumentMapper = argumentMapper;
    }

    ISyntacticAttributeRecorder<ISyntacticCombinedAttributeRecord> ISyntacticCombinedAttributeRecorderFactory.Create() => Factory.Create<ISyntacticCombinedAttributeRecord, ISyntacticCombinedAttributeRecordBuilder>(ArgumentMapper, new CombinedAttributeDataBuilder());

    private sealed class CombinedAttributeDataBuilder : ISyntacticCombinedAttributeRecordBuilder
    {
        private CombinedAttributeData Target { get; } = new();

        ISyntacticCombinedAttributeRecord IRecordBuilder<ISyntacticCombinedAttributeRecord>.Build() => Target;

        void ISyntacticCombinedAttributeRecordBuilder.WithT1Syntax(ExpressionSyntax syntax)
        {
            Target.T1Syntax = syntax;
            Target.T1SyntaxRecorded = true;
        }

        void ISyntacticCombinedAttributeRecordBuilder.WithT2Syntax(ExpressionSyntax syntax)
        {
            Target.T2Syntax = syntax;
            Target.T2SyntaxRecorded = true;
        }

        void ISyntacticCombinedAttributeRecordBuilder.WithSimpleValueSyntax(ExpressionSyntax syntax)
        {
            Target.SimpleValueSyntax = syntax;
            Target.SimpleValueSyntaxRecorded = true;
        }

        void ISyntacticCombinedAttributeRecordBuilder.WithArrayValueSyntax(ExpressionSyntax syntax)
        {
            Target.ArrayValueSyntax = syntax;
            Target.ArrayValueSyntaxRecorded = true;
        }

        void ISyntacticCombinedAttributeRecordBuilder.WithParamsValueSyntax(OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
        {
            Target.ParamsValueSyntax = syntax;
            Target.ParamsValueSyntaxRecorded = true;
        }

        void ISyntacticCombinedAttributeRecordBuilder.WithSimpleNamedValueSyntax(ExpressionSyntax syntax)
        {
            Target.SimpleNamedValueSyntax = syntax;
            Target.SimpleNamedValueSyntaxRecorded = true;
        }

        void ISyntacticCombinedAttributeRecordBuilder.WithArrayNamedValueSyntax(ExpressionSyntax syntax)
        {
            Target.ArrayNamedValueSyntax = syntax;
            Target.ArrayNamedValueSyntaxRecorded = true;
        }

        private sealed class CombinedAttributeData : ISyntacticCombinedAttributeRecord
        {
            public ExpressionSyntax? T1Syntax { get; set; }
            public bool T1SyntaxRecorded { get; set; }

            public ExpressionSyntax? T2Syntax { get; set; }
            public bool T2SyntaxRecorded { get; set; }

            public ExpressionSyntax? SimpleValueSyntax { get; set; }
            public bool SimpleValueSyntaxRecorded { get; set; }

            public ExpressionSyntax? ArrayValueSyntax { get; set; }
            public bool ArrayValueSyntaxRecorded { get; set; }

            public OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> ParamsValueSyntax { get; set; }
            public bool ParamsValueSyntaxRecorded { get; set; }

            public ExpressionSyntax? SimpleNamedValueSyntax { get; set; }
            public bool SimpleNamedValueSyntaxRecorded { get; set; }

            public ExpressionSyntax? ArrayNamedValueSyntax { get; set; }
            public bool ArrayNamedValueSyntaxRecorded { get; set; }
        }
    }
}
