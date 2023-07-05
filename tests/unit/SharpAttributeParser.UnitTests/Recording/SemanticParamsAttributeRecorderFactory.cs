namespace SharpAttributeParser.Recording;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SemanticParamsAttributeRecorderFactory : ISemanticParamsAttributeRecorderFactory
{
    private ISemanticAttributeRecorderFactory Factory { get; }
    private ISemanticAttributeMapper<IParamsAttributeDataBuilder> ArgumentMapper { get; }

    public SemanticParamsAttributeRecorderFactory(ISemanticAttributeRecorderFactory factory, ISemanticAttributeMapper<IParamsAttributeDataBuilder> argumentMapper)
    {
        Factory = factory;
        ArgumentMapper = argumentMapper;
    }

    ISemanticAttributeRecorder<IParamsAttributeData> ISemanticParamsAttributeRecorderFactory.Create() => Factory.Create<IParamsAttributeData, IParamsAttributeDataBuilder>(ArgumentMapper, new ParamsAttributeDataBuilder());

    private sealed class ParamsAttributeDataBuilder : IParamsAttributeDataBuilder
    {
        private ParamsAttributeData Target { get; } = new();

        IParamsAttributeData IAttributeDataBuilder<IParamsAttributeData>.Build() => Target;

        void IParamsAttributeDataBuilder.WithValue(IReadOnlyList<object?>? value, CollectionLocation location)
        {
            Target.Value = value;
            Target.ValueRecorded = true;
            Target.ValueLocation = location;
        }

        private sealed class ParamsAttributeData : IParamsAttributeData
        {
            public IReadOnlyList<object?>? Value { get; set; }
            public bool ValueRecorded { get; set; }
            public CollectionLocation ValueLocation { get; set; } = CollectionLocation.None;
        }
    }
}
