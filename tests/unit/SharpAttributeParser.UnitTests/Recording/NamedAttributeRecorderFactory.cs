namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class NamedAttributeRecorderFactory : INamedAttributeRecorderFactory
{
    private IAttributeRecorderFactory Factory { get; }
    private IAttributeMapper<INamedAttributeRecordBuilder> ArgumentMapper { get; }

    public NamedAttributeRecorderFactory(IAttributeRecorderFactory factory, IAttributeMapper<INamedAttributeRecordBuilder> argumentMapper)
    {
        Factory = factory;
        ArgumentMapper = argumentMapper;
    }

    IAttributeRecorder<INamedAttributeRecord> INamedAttributeRecorderFactory.Create() => Factory.Create<INamedAttributeRecord, INamedAttributeRecordBuilder>(ArgumentMapper, new NamedAttributeRecordBuilder());

    private sealed class NamedAttributeRecordBuilder : INamedAttributeRecordBuilder
    {
        private NamedAttributeRecord Target { get; } = new();

        INamedAttributeRecord IRecordBuilder<INamedAttributeRecord>.Build() => Target;
        ISemanticNamedAttributeRecord IRecordBuilder<ISemanticNamedAttributeRecord>.Build() => Target;
        ISyntacticNamedAttributeRecord IRecordBuilder<ISyntacticNamedAttributeRecord>.Build() => Target;

        void ISemanticNamedAttributeRecordBuilder.WithSimpleValue(object? value)
        {
            Target.SimpleValue = value;
            Target.SimpleValueRecorded = true;
        }

        void ISemanticNamedAttributeRecordBuilder.WithArrayValue(IReadOnlyList<object?>? value)
        {
            Target.ArrayValue = value;
            Target.ArrayValueRecorded = true;
        }

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

        private sealed class NamedAttributeRecord : INamedAttributeRecord
        {
            public object? SimpleValue { get; set; }
            public bool SimpleValueRecorded { get; set; }

            public IReadOnlyList<object?>? ArrayValue { get; set; }
            public bool ArrayValueRecorded { get; set; }

            public ExpressionSyntax? SimpleValueSyntax { get; set; }
            public bool SimpleValueSyntaxRecorded { get; set; }

            public ExpressionSyntax? ArrayValueSyntax { get; set; }
            public bool ArrayValueSyntaxRecorded { get; set; }
        }
    }
}
