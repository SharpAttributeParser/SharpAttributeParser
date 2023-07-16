namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class CombinedAttributeRecorderFactory : ICombinedAttributeRecorderFactory
{
    private IAttributeRecorderFactory Factory { get; }
    private IAttributeMapper<ICombinedAttributeRecordBuilder> ArgumentMapper { get; }

    public CombinedAttributeRecorderFactory(IAttributeRecorderFactory factory, IAttributeMapper<ICombinedAttributeRecordBuilder> argumentMapper)
    {
        Factory = factory;
        ArgumentMapper = argumentMapper;
    }

    IAttributeRecorder<ICombinedAttributeRecord> ICombinedAttributeRecorderFactory.Create() => Factory.Create<ICombinedAttributeRecord, ICombinedAttributeRecordBuilder>(ArgumentMapper, new CombinedAttributeRecordBuilder());

    private sealed class CombinedAttributeRecordBuilder : ICombinedAttributeRecordBuilder
    {
        private CombinedAttributeRecord Target { get; } = new();

        ICombinedAttributeRecord IRecordBuilder<ICombinedAttributeRecord>.Build() => Target;
        ISemanticCombinedAttributeRecord IRecordBuilder<ISemanticCombinedAttributeRecord>.Build() => Target;
        ISyntacticCombinedAttributeRecord IRecordBuilder<ISyntacticCombinedAttributeRecord>.Build() => Target;

        void ISemanticCombinedAttributeRecordBuilder.WithT1(ITypeSymbol t1)
        {
            Target.T1 = t1;
            Target.T1Recorded = true;
        }

        void ISemanticCombinedAttributeRecordBuilder.WithT2(ITypeSymbol t2)
        {
            Target.T2 = t2;
            Target.T2Recorded = true;
        }

        void ISemanticCombinedAttributeRecordBuilder.WithSimpleValue(object? value)
        {
            Target.SimpleValue = value;
            Target.SimpleValueRecorded = true;
        }

        void ISemanticCombinedAttributeRecordBuilder.WithArrayValue(IReadOnlyList<object?>? value)
        {
            Target.ArrayValue = value;
            Target.ArrayValueRecorded = true;
        }

        void ISemanticCombinedAttributeRecordBuilder.WithParamsValue(IReadOnlyList<object?>? value)
        {
            Target.ParamsValue = value;
            Target.ParamsValueRecorded = true;
        }

        void ISemanticCombinedAttributeRecordBuilder.WithSimpleNamedValue(object? value)
        {
            Target.SimpleNamedValue = value;
            Target.SimpleNamedValueRecorded = true;
        }

        void ISemanticCombinedAttributeRecordBuilder.WithArrayNamedValue(IReadOnlyList<object?>? value)
        {
            Target.ArrayNamedValue = value;
            Target.ArrayNamedValueRecorded = true;
        }

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

        private sealed class CombinedAttributeRecord : ICombinedAttributeRecord
        {
            public ITypeSymbol? T1 { get; set; }
            public bool T1Recorded { get; set; }

            public ITypeSymbol? T2 { get; set; }
            public bool T2Recorded { get; set; }

            public object? SimpleValue { get; set; }
            public bool SimpleValueRecorded { get; set; }

            public IReadOnlyList<object?>? ArrayValue { get; set; }
            public bool ArrayValueRecorded { get; set; }

            public IReadOnlyList<object?>? ParamsValue { get; set; }
            public bool ParamsValueRecorded { get; set; }

            public object? SimpleNamedValue { get; set; }
            public bool SimpleNamedValueRecorded { get; set; }

            public IReadOnlyList<object?>? ArrayNamedValue { get; set; }
            public bool ArrayNamedValueRecorded { get; set; }

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
