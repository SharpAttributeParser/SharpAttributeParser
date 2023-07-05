namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SemanticNamedAttributeRecorderFactory : ISemanticNamedAttributeRecorderFactory
{
    private ISemanticAttributeRecorderFactory Factory { get; }
    private ISemanticAttributeMapper<INamedAttributeDataBuilder> ArgumentMapper { get; }

    public SemanticNamedAttributeRecorderFactory(ISemanticAttributeRecorderFactory factory, ISemanticAttributeMapper<INamedAttributeDataBuilder> argumentMapper)
    {
        Factory = factory;
        ArgumentMapper = argumentMapper;
    }

    ISemanticAttributeRecorder<INamedAttributeData> ISemanticNamedAttributeRecorderFactory.Create() => Factory.Create<INamedAttributeData, INamedAttributeDataBuilder>(ArgumentMapper, new NamedAttributeDataBuilder());

    private sealed class NamedAttributeDataBuilder : INamedAttributeDataBuilder
    {
        private NamedAttributeData Target { get; } = new();

        INamedAttributeData IAttributeDataBuilder<INamedAttributeData>.Build() => Target;

        void INamedAttributeDataBuilder.WithSimpleValue(object? value, Location location)
        {
            Target.SimpleValue = value;
            Target.SimpleValueRecorded = true;
            Target.SimpleValueLocation = location;
        }

        void INamedAttributeDataBuilder.WithArrayValue(IReadOnlyList<object?>? value, CollectionLocation location)
        {
            Target.ArrayValue = value;
            Target.ArrayValueRecorded = true;
            Target.ArrayValueLocation = location;
        }

        private sealed class NamedAttributeData : INamedAttributeData
        {
            public object? SimpleValue { get; set; }
            public bool SimpleValueRecorded { get; set; }
            public Location SimpleValueLocation { get; set; } = Location.None;

            public IReadOnlyList<object?>? ArrayValue { get; set; }
            public bool ArrayValueRecorded { get; set; }
            public CollectionLocation ArrayValueLocation { get; set; } = CollectionLocation.None;
        }
    }
}
