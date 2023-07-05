namespace SharpAttributeParser.Recording;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SemanticArrayConstructorAttributeRecorderFactory : ISemanticArrayConstructorAttributeRecorderFactory
{
    private ISemanticAttributeRecorderFactory Factory { get; }
    private ISemanticAttributeMapper<IArrayConstructorAttributeDataBuilder> ArgumentMapper { get; }

    public SemanticArrayConstructorAttributeRecorderFactory(ISemanticAttributeRecorderFactory factory, ISemanticAttributeMapper<IArrayConstructorAttributeDataBuilder> argumentMapper)
    {
        Factory = factory;
        ArgumentMapper = argumentMapper;
    }

    ISemanticAttributeRecorder<IArrayConstructorAttributeData> ISemanticArrayConstructorAttributeRecorderFactory.Create() => Factory.Create<IArrayConstructorAttributeData, IArrayConstructorAttributeDataBuilder>(ArgumentMapper, new ArrayConstructorAttributeDataBuilder());

    private sealed class ArrayConstructorAttributeDataBuilder : IArrayConstructorAttributeDataBuilder
    {
        private ArrayConstructorAttributeData Target { get; } = new();

        IArrayConstructorAttributeData IAttributeDataBuilder<IArrayConstructorAttributeData>.Build() => Target;

        void IArrayConstructorAttributeDataBuilder.WithValue(IReadOnlyList<object?>? value, CollectionLocation location)
        {
            Target.Value = value;
            Target.ValueRecorded = true;
            Target.ValueLocation = location;
        }

        private sealed class ArrayConstructorAttributeData : IArrayConstructorAttributeData
        {
            public IReadOnlyList<object?>? Value { get; set; }
            public bool ValueRecorded { get; set; }
            public CollectionLocation ValueLocation { get; set; } = CollectionLocation.None;
        }
    }
}
