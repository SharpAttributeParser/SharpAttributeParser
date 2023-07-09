namespace SharpAttributeParser.Recording;

using SharpAttributeParser;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SemanticSimpleConstructorAttributeRecorderFactory : ISemanticSimpleConstructorAttributeRecorderFactory
{
    private ISemanticAttributeRecorderFactory Factory { get; }
    private ISemanticAttributeMapper<ISemanticSimpleConstructorAttributeRecordBuilder> ArgumentMapper { get; }

    public SemanticSimpleConstructorAttributeRecorderFactory(ISemanticAttributeRecorderFactory factory, ISemanticAttributeMapper<ISemanticSimpleConstructorAttributeRecordBuilder> argumentMapper)
    {
        Factory = factory;
        ArgumentMapper = argumentMapper;
    }

    ISemanticAttributeRecorder<ISemanticSimpleConstructorAttributeRecord> ISemanticSimpleConstructorAttributeRecorderFactory.Create() => Factory.Create<ISemanticSimpleConstructorAttributeRecord, ISemanticSimpleConstructorAttributeRecordBuilder>(ArgumentMapper, new SimpleConstructorAttributeDataBuilder());

    private sealed class SimpleConstructorAttributeDataBuilder : ISemanticSimpleConstructorAttributeRecordBuilder
    {
        private SimpleConstructorAttributeData Target { get; } = new();

        ISemanticSimpleConstructorAttributeRecord IRecordBuilder<ISemanticSimpleConstructorAttributeRecord>.Build() => Target;

        void ISemanticSimpleConstructorAttributeRecordBuilder.WithValue(object? value)
        {
            Target.Value = value;
            Target.ValueRecorded = true;
        }

        private sealed class SimpleConstructorAttributeData : ISemanticSimpleConstructorAttributeRecord
        {
            public object? Value { get; set; }
            public bool ValueRecorded { get; set; }
        }
    }
}
