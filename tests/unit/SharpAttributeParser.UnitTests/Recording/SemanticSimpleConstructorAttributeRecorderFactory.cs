namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis;

using SharpAttributeParser;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SemanticSimpleConstructorAttributeRecorderFactory : ISemanticSimpleConstructorAttributeRecorderFactory
{
    private ISemanticAttributeRecorderFactory Factory { get; }
    private ISemanticAttributeMapper<ISimpleConstructorAttributeDataBuilder> ArgumentMapper { get; }

    public SemanticSimpleConstructorAttributeRecorderFactory(ISemanticAttributeRecorderFactory factory, ISemanticAttributeMapper<ISimpleConstructorAttributeDataBuilder> argumentMapper)
    {
        Factory = factory;
        ArgumentMapper = argumentMapper;
    }

    ISemanticAttributeRecorder<ISimpleConstructorAttributeData> ISemanticSimpleConstructorAttributeRecorderFactory.Create() => Factory.Create<ISimpleConstructorAttributeData, ISimpleConstructorAttributeDataBuilder>(ArgumentMapper, new SimpleConstructorAttributeDataBuilder());

    private sealed class SimpleConstructorAttributeDataBuilder : ISimpleConstructorAttributeDataBuilder
    {
        private SimpleConstructorAttributeData Target { get; } = new();

        ISimpleConstructorAttributeData IAttributeDataBuilder<ISimpleConstructorAttributeData>.Build() => Target;

        void ISimpleConstructorAttributeDataBuilder.WithValue(object? value, Location location)
        {
            Target.Value = value;
            Target.ValueRecorded = true;
            Target.ValueLocation = location;
        }

        private sealed class SimpleConstructorAttributeData : ISimpleConstructorAttributeData
        {
            public object? Value { get; set; }
            public bool ValueRecorded { get; set; }
            public Location ValueLocation { get; set; } = Location.None;
        }
    }
}
