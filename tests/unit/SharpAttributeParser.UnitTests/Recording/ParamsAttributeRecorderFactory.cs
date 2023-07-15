namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class ParamsAttributeRecorderFactory : IParamsAttributeRecorderFactory
{
    private IAttributeRecorderFactory Factory { get; }
    private IAttributeMapper<IParamsAttributeRecordBuilder> ArgumentMapper { get; }

    public ParamsAttributeRecorderFactory(IAttributeRecorderFactory factory, IAttributeMapper<IParamsAttributeRecordBuilder> argumentMapper)
    {
        Factory = factory;
        ArgumentMapper = argumentMapper;
    }

    IAttributeRecorder<IParamsAttributeRecord> IParamsAttributeRecorderFactory.Create() => Factory.Create<IParamsAttributeRecord, IParamsAttributeRecordBuilder>(ArgumentMapper, new ParamsAttributeDataBuilder());

    private sealed class ParamsAttributeDataBuilder : IParamsAttributeRecordBuilder
    {
        private ParamsAttributeData Target { get; } = new();

        IParamsAttributeRecord IRecordBuilder<IParamsAttributeRecord>.Build() => Target;
        ISemanticParamsAttributeRecord IRecordBuilder<ISemanticParamsAttributeRecord>.Build() => Target;
        ISyntacticParamsAttributeRecord IRecordBuilder<ISyntacticParamsAttributeRecord>.Build() => Target;

        void ISemanticParamsAttributeRecordBuilder.WithValue(IReadOnlyList<object?>? value)
        {
            Target.Value = value;
            Target.ValueRecorded = true;
        }

        void ISyntacticParamsAttributeRecordBuilder.WithValueSyntax(OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax)
        {
            Target.ValueSyntax = syntax;
            Target.ValueSyntaxRecorded = true;
        }

        private sealed class ParamsAttributeData : IParamsAttributeRecord
        {
            public IReadOnlyList<object?>? Value { get; set; }
            public bool ValueRecorded { get; set; }

            public OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> ValueSyntax { get; set; }
            public bool ValueSyntaxRecorded { get; set; }
        }
    }
}
