namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SimpleConstructorAttributeRecorderFactory : ISimpleConstructorAttributeRecorderFactory
{
    private IAttributeRecorderFactory Factory { get; }
    private IAttributeMapper<ISimpleConstructorAttributeRecordBuilder> ArgumentMapper { get; }

    public SimpleConstructorAttributeRecorderFactory(IAttributeRecorderFactory factory, IAttributeMapper<ISimpleConstructorAttributeRecordBuilder> argumentMapper)
    {
        Factory = factory;
        ArgumentMapper = argumentMapper;
    }

    IAttributeRecorder<ISimpleConstructorAttributeRecord> ISimpleConstructorAttributeRecorderFactory.Create() => Factory.Create<ISimpleConstructorAttributeRecord, ISimpleConstructorAttributeRecordBuilder>(ArgumentMapper, new SimpleConstructorAttributeRecordBuilder());

    private sealed class SimpleConstructorAttributeRecordBuilder : ISimpleConstructorAttributeRecordBuilder
    {
        private SimpleConstructorAttributeRecord Target { get; } = new();

        ISimpleConstructorAttributeRecord IRecordBuilder<ISimpleConstructorAttributeRecord>.Build() => Target;
        ISemanticSimpleConstructorAttributeRecord IRecordBuilder<ISemanticSimpleConstructorAttributeRecord>.Build() => Target;
        ISyntacticSimpleConstructorAttributeRecord IRecordBuilder<ISyntacticSimpleConstructorAttributeRecord>.Build() => Target;

        void ISemanticSimpleConstructorAttributeRecordBuilder.WithValue(object? value)
        {
            Target.Value = value;
            Target.ValueRecorded = true;
        }

        void ISyntacticSimpleConstructorAttributeRecordBuilder.WithValueSyntax(ExpressionSyntax syntax)
        {
            Target.ValueSyntax = syntax;
            Target.ValueSyntaxRecorded = true;
        }

        private sealed class SimpleConstructorAttributeRecord : ISimpleConstructorAttributeRecord
        {
            public object? Value { get; set; }
            public bool ValueRecorded { get; set; }

            public ExpressionSyntax? ValueSyntax { get; set; }
            public bool ValueSyntaxRecorded { get; set; }
        }
    }
}
