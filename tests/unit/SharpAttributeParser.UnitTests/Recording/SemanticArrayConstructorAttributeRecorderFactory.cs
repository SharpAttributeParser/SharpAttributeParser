namespace SharpAttributeParser.Recording;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SemanticArrayConstructorAttributeRecorderFactory : ISemanticArrayConstructorAttributeRecorderFactory
{
    private ISemanticAttributeRecorderFactory Factory { get; }
    private ISemanticAttributeMapper<ISemanticArrayConstructorAttributeRecordBuilder> ArgumentMapper { get; }

    public SemanticArrayConstructorAttributeRecorderFactory(ISemanticAttributeRecorderFactory factory, ISemanticAttributeMapper<ISemanticArrayConstructorAttributeRecordBuilder> argumentMapper)
    {
        Factory = factory;
        ArgumentMapper = argumentMapper;
    }

    ISemanticAttributeRecorder<ISemanticArrayConstructorAttributeRecord> ISemanticArrayConstructorAttributeRecorderFactory.Create() => Factory.Create<ISemanticArrayConstructorAttributeRecord, ISemanticArrayConstructorAttributeRecordBuilder>(ArgumentMapper, new ArrayConstructorAttributeDataBuilder());

    private sealed class ArrayConstructorAttributeDataBuilder : ISemanticArrayConstructorAttributeRecordBuilder
    {
        private ArrayConstructorAttributeData Target { get; } = new();

        ISemanticArrayConstructorAttributeRecord IRecordBuilder<ISemanticArrayConstructorAttributeRecord>.Build() => Target;

        void ISemanticArrayConstructorAttributeRecordBuilder.WithValue(IReadOnlyList<object?>? value)
        {
            Target.Value = value;
            Target.ValueRecorded = true;
        }

        private sealed class ArrayConstructorAttributeData : ISemanticArrayConstructorAttributeRecord
        {
            public IReadOnlyList<object?>? Value { get; set; }
            public bool ValueRecorded { get; set; }
        }
    }
}
