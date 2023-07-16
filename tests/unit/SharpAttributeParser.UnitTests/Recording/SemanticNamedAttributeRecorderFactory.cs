namespace SharpAttributeParser.Recording;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SemanticNamedAttributeRecorderFactory : ISemanticNamedAttributeRecorderFactory
{
    private ISemanticAttributeRecorderFactory Factory { get; }
    private ISemanticAttributeMapper<ISemanticNamedAttributeRecordBuilder> ArgumentMapper { get; }

    public SemanticNamedAttributeRecorderFactory(ISemanticAttributeRecorderFactory factory, ISemanticAttributeMapper<ISemanticNamedAttributeRecordBuilder> argumentMapper)
    {
        Factory = factory;
        ArgumentMapper = argumentMapper;
    }

    ISemanticAttributeRecorder<ISemanticNamedAttributeRecord> ISemanticNamedAttributeRecorderFactory.Create() => Factory.Create<ISemanticNamedAttributeRecord, ISemanticNamedAttributeRecordBuilder>(ArgumentMapper, new NamedAttributeRecordBuilder());

    private sealed class NamedAttributeRecordBuilder : ISemanticNamedAttributeRecordBuilder
    {
        private NamedAttributeRecord Target { get; } = new();

        ISemanticNamedAttributeRecord IRecordBuilder<ISemanticNamedAttributeRecord>.Build() => Target;

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

        private sealed class NamedAttributeRecord : ISemanticNamedAttributeRecord
        {
            public object? SimpleValue { get; set; }
            public bool SimpleValueRecorded { get; set; }

            public IReadOnlyList<object?>? ArrayValue { get; set; }
            public bool ArrayValueRecorded { get; set; }
        }
    }
}
